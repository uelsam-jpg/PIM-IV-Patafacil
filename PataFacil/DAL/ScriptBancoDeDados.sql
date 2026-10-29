-- ============================================================
-- PATAFÁCIL — SCRIPT COMPLETO DO BANCO DE DADOS (PIM IV)
-- Etapa 7: Projeto do Banco de Dados — MySQL 8
-- ============================================================
-- Este é o banco usado pelas APIs, pela aplicação Web e pelo Mobile.
-- A própria API aplica este script ao iniciar (DAL/InicializadorBanco.cs),
-- portanto ele pode ser executado várias vezes sem erro (idempotente).
--
-- Execução manual, se desejar:
--   mysql -u root -p < ScriptBancoDeDados.sql
--
-- Convenção: os enums do C# são gravados como INT (os valores estão
-- comentados em cada coluna).

CREATE DATABASE IF NOT EXISTS patafacil
  DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE patafacil;

-- ============================================================
-- 1. TABELAS (MODELO FÍSICO)
-- ============================================================

-- Tabela 1: Tutores (especialização de Pessoa)
CREATE TABLE IF NOT EXISTS tutores (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(120) NOT NULL,
    Cpf CHAR(11) NOT NULL,
    Email VARCHAR(160) NOT NULL,
    Telefone VARCHAR(20) NOT NULL DEFAULT '',
    Logradouro VARCHAR(160) NOT NULL DEFAULT '',
    Numero VARCHAR(10) NOT NULL DEFAULT '',
    Bairro VARCHAR(80) NOT NULL DEFAULT '',
    Cidade VARCHAR(80) NOT NULL DEFAULT '',
    Uf CHAR(2) NOT NULL DEFAULT '',
    Cep VARCHAR(9) NOT NULL DEFAULT '',
    DataCadastro DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    SenhaHash VARCHAR(100) NOT NULL,          -- hash BCrypt (nunca a senha em texto puro)
    Ativo TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_tutores PRIMARY KEY (Id),
    CONSTRAINT UQ_tutores_email UNIQUE (Email),
    CONSTRAINT UQ_tutores_cpf UNIQUE (Cpf)
) ENGINE=InnoDB;

-- Tabela 2: Pets
CREATE TABLE IF NOT EXISTS pets (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(80) NOT NULL,
    Especie INT NOT NULL,                     -- 0=Cachorro, 1=Gato, 2=Ave, 3=Roedor, 4=Outro
    Raca VARCHAR(80) NOT NULL DEFAULT '',
    Sexo INT NOT NULL,                        -- 0=Macho, 1=Femea
    DataNascimento DATETIME(6) NOT NULL,
    PesoKg DECIMAL(6,2) NOT NULL DEFAULT 0,
    FotoUrl VARCHAR(255) NOT NULL DEFAULT '',
    Observacoes TEXT NOT NULL,
    Ativo TINYINT(1) NOT NULL DEFAULT 1,
    TutorId INT NOT NULL,
    CONSTRAINT PK_pets PRIMARY KEY (Id),
    CONSTRAINT FK_pets_tutores FOREIGN KEY (TutorId) REFERENCES tutores(Id),
    CONSTRAINT CK_pets_especie CHECK (Especie BETWEEN 0 AND 4),
    CONSTRAINT CK_pets_sexo CHECK (Sexo IN (0, 1)),
    INDEX idx_pets_tutor (TutorId)
) ENGINE=InnoDB;

-- Tabela 3: Funcionarios (especialização de Pessoa)
CREATE TABLE IF NOT EXISTS funcionarios (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(120) NOT NULL,
    Cpf CHAR(11) NOT NULL,
    Email VARCHAR(160) NOT NULL,
    Cargo INT NOT NULL,                       -- 0=Veterinario, 1=Atendente, 2=Gerente, 3=Tosador (Banho e Tosa)
    SenhaHash VARCHAR(100) NOT NULL,          -- hash BCrypt
    DataAdmissao DATETIME(6) NOT NULL,
    Ativo TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_funcionarios PRIMARY KEY (Id),
    CONSTRAINT UQ_funcionarios_email UNIQUE (Email),
    CONSTRAINT UQ_funcionarios_cpf UNIQUE (Cpf),
    CONSTRAINT CK_funcionarios_cargo CHECK (Cargo BETWEEN 0 AND 3)
) ENGINE=InnoDB;

-- Tabela 4: Servicos
CREATE TABLE IF NOT EXISTS servicos (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(80) NOT NULL,
    Descricao VARCHAR(255) NOT NULL DEFAULT '',
    DuracaoMinutos INT NOT NULL,
    Preco DECIMAL(10,2) NOT NULL,
    Ativo TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_servicos PRIMARY KEY (Id),
    CONSTRAINT CK_servicos_duracao CHECK (DuracaoMinutos > 0),
    CONSTRAINT CK_servicos_preco CHECK (Preco >= 0)
) ENGINE=InnoDB;

-- Tabela 5: Agendamentos
CREATE TABLE IF NOT EXISTS agendamentos (
    Id INT NOT NULL AUTO_INCREMENT,
    DataHoraInicio DATETIME(6) NOT NULL,
    DataHoraFim DATETIME(6) NOT NULL,
    Observacoes TEXT NULL,
    DataCriacao DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    IdPet INT NOT NULL,
    IdServico INT NOT NULL,
    IdFuncionario INT NOT NULL,
    Status INT NOT NULL DEFAULT 0,            -- 0=PENDENTE, 1=CONFIRMADO, 2=CONCLUIDO, 3=CANCELADO
    ObservacoesAtendimento TEXT NULL,         -- anotações internas da equipe sobre o pet
    DataChegada DATETIME(6) NULL,             -- preenchida quando o cliente chega ao balcão ("em processo")
    ValorPago DECIMAL(10,2) NULL,             -- baixa no balcão
    FormaPagamento INT NULL,                  -- 0=Dinheiro, 1=CartaoCredito, 2=CartaoDebito, 3=Pix
    DataPagamento DATETIME(6) NULL,
    CONSTRAINT PK_agendamentos PRIMARY KEY (Id),
    CONSTRAINT FK_agendamentos_pets FOREIGN KEY (IdPet) REFERENCES pets(Id),
    CONSTRAINT FK_agendamentos_servicos FOREIGN KEY (IdServico) REFERENCES servicos(Id),
    CONSTRAINT FK_agendamentos_funcionarios FOREIGN KEY (IdFuncionario) REFERENCES funcionarios(Id),
    CONSTRAINT CK_agendamentos_status CHECK (Status BETWEEN 0 AND 3),
    CONSTRAINT CK_agendamentos_periodo CHECK (DataHoraFim > DataHoraInicio),
    -- Índices de otimização: checagem de conflito (RN01), agenda do dia/semana e filtros por status
    INDEX idx_agendamentos_funcionario_horario (IdFuncionario, DataHoraInicio),
    INDEX idx_agendamentos_data (DataHoraInicio),
    INDEX idx_agendamentos_status (Status)
) ENGINE=InnoDB;

-- Tabela 6: Historicos (Histórico Clínico)
CREATE TABLE IF NOT EXISTS historicos (
    Id INT NOT NULL AUTO_INCREMENT,
    DataAtendimento DATETIME(6) NOT NULL,
    Tipo INT NOT NULL,                        -- 0=Consulta, 1=Vacina, 2=Cirurgia, 3=Retorno, 4=Exame
    Descricao TEXT NOT NULL,
    Diagnostico TEXT NULL,
    Prescricao TEXT NULL,
    PesoRegistro DECIMAL(6,2) NOT NULL DEFAULT 0,
    ProximaVacina DATETIME(6) NULL,
    IdPet INT NOT NULL,
    IdFuncionario INT NOT NULL,
    IdAgendamento INT NULL,                   -- opcional: nem todo registro nasce de um agendamento
    CONSTRAINT PK_historicos PRIMARY KEY (Id),
    CONSTRAINT FK_historicos_pets FOREIGN KEY (IdPet) REFERENCES pets(Id),
    CONSTRAINT FK_historicos_funcionarios FOREIGN KEY (IdFuncionario) REFERENCES funcionarios(Id),
    CONSTRAINT FK_historicos_agendamentos FOREIGN KEY (IdAgendamento) REFERENCES agendamentos(Id),
    CONSTRAINT CK_historicos_tipo CHECK (Tipo BETWEEN 0 AND 4),
    INDEX idx_historicos_pet (IdPet, DataAtendimento)
) ENGINE=InnoDB;

-- Tabela 7: Produtos
CREATE TABLE IF NOT EXISTS produtos (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(120) NOT NULL,
    Descricao VARCHAR(255) NOT NULL DEFAULT '',
    Categoria INT NOT NULL DEFAULT 0,         -- 0=Alimento, 1=Medicamento, 2=Acessorio, 3=Higiene, 4=Outro
    Marca VARCHAR(80) NOT NULL DEFAULT '',
    PrecoCusto DECIMAL(10,2) NOT NULL DEFAULT 0,
    PrecoVenda DECIMAL(10,2) NOT NULL,
    UnidadeMedida VARCHAR(10) NOT NULL DEFAULT '',
    CodigoBarras VARCHAR(20) NOT NULL DEFAULT '',
    Ativo TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_produtos PRIMARY KEY (Id),
    CONSTRAINT CK_produtos_categoria CHECK (Categoria BETWEEN 0 AND 4),
    CONSTRAINT CK_produtos_preco CHECK (PrecoVenda >= 0 AND PrecoCusto >= 0)
) ENGINE=InnoDB;

-- Tabela 8: Estoques (um registro de saldo por produto)
CREATE TABLE IF NOT EXISTS estoques (
    Id INT NOT NULL AUTO_INCREMENT,
    QuantidadeAtual INT NOT NULL DEFAULT 0,
    PontoReposicao INT NOT NULL DEFAULT 0,
    QuantidadeReposicao INT NOT NULL DEFAULT 0,
    UltimaAtualizacao DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    IdProduto INT NOT NULL,
    CONSTRAINT PK_estoques PRIMARY KEY (Id),
    CONSTRAINT FK_estoques_produtos FOREIGN KEY (IdProduto) REFERENCES produtos(Id),
    CONSTRAINT UQ_estoques_produto UNIQUE (IdProduto),
    -- Última barreira contra venda sem saldo: o banco recusa estoque negativo
    CONSTRAINT CK_estoques_saldo CHECK (QuantidadeAtual >= 0)
) ENGINE=InnoDB;

-- Tabela 9: Vendas
CREATE TABLE IF NOT EXISTS vendas (
    Id INT NOT NULL AUTO_INCREMENT,
    DataVenda DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    Total DECIMAL(10,2) NOT NULL DEFAULT 0,
    FormaPagamento INT NOT NULL,              -- 0=Dinheiro, 1=CartaoCredito, 2=CartaoDebito, 3=Pix
    Observacoes TEXT NULL,
    IdTutor INT NULL,                         -- opcional: venda de balcão sem cliente cadastrado
    IdFuncionario INT NOT NULL,
    CONSTRAINT PK_vendas PRIMARY KEY (Id),
    CONSTRAINT FK_vendas_tutores FOREIGN KEY (IdTutor) REFERENCES tutores(Id),
    CONSTRAINT FK_vendas_funcionarios FOREIGN KEY (IdFuncionario) REFERENCES funcionarios(Id),
    CONSTRAINT CK_vendas_pagamento CHECK (FormaPagamento BETWEEN 0 AND 3),
    INDEX idx_vendas_data (DataVenda)
) ENGINE=InnoDB;

-- Tabela 10: ItensVenda
CREATE TABLE IF NOT EXISTS itensvenda (
    Id INT NOT NULL AUTO_INCREMENT,
    Quantidade INT NOT NULL,
    PrecoUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    IdVenda INT NOT NULL,
    IdProduto INT NOT NULL,
    CONSTRAINT PK_itensvenda PRIMARY KEY (Id),
    CONSTRAINT FK_itensvenda_vendas FOREIGN KEY (IdVenda) REFERENCES vendas(Id),
    CONSTRAINT FK_itensvenda_produtos FOREIGN KEY (IdProduto) REFERENCES produtos(Id),
    CONSTRAINT CK_itensvenda_quantidade CHECK (Quantidade > 0),
    INDEX idx_itensvenda_produto (IdProduto)
) ENGINE=InnoDB;

-- ============================================================
-- 2. TABELAS DE APOIO (AUDITORIA E ALERTAS)
-- ============================================================

-- Log de auditoria do status do agendamento (apoia a RN04)
CREATE TABLE IF NOT EXISTS log_agendamento_status (
    Id INT NOT NULL AUTO_INCREMENT,
    IdAgendamento INT NOT NULL,
    StatusAnterior INT NULL,
    StatusNovo INT NOT NULL,
    DataMudanca DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT PK_log_agendamento_status PRIMARY KEY (Id),
    CONSTRAINT FK_log_status_agendamento FOREIGN KEY (IdAgendamento) REFERENCES agendamentos(Id)
) ENGINE=InnoDB;

-- Alerta automático de reposição (apoia a RN15)
CREATE TABLE IF NOT EXISTS alerta_reposicao (
    Id INT NOT NULL AUTO_INCREMENT,
    IdProduto INT NOT NULL,
    QuantidadeAtual INT NOT NULL,
    PontoReposicao INT NOT NULL,
    DataAlerta DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    Resolvido TINYINT(1) NOT NULL DEFAULT 0,
    CONSTRAINT PK_alerta_reposicao PRIMARY KEY (Id),
    CONSTRAINT FK_alerta_produto FOREIGN KEY (IdProduto) REFERENCES produtos(Id)
) ENGINE=InnoDB;

-- Caixa de E-mail: e-mails automáticos enviados pelo sistema (RF09)
CREATE TABLE IF NOT EXISTS emails (
    Id INT NOT NULL AUTO_INCREMENT,
    Destinatario VARCHAR(160) NOT NULL,
    Assunto VARCHAR(200) NOT NULL,
    Corpo TEXT NOT NULL,
    DataEnvio DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    Lida TINYINT(1) NOT NULL DEFAULT 0,
    CONSTRAINT PK_emails PRIMARY KEY (Id),
    INDEX idx_emails_destinatario (Destinatario, DataEnvio)
) ENGINE=InnoDB;

-- Entradas de mercadoria (compra do fornecedor): nota, custo e validade de cada lote recebido
CREATE TABLE IF NOT EXISTS entradas_estoque (
    Id INT NOT NULL AUTO_INCREMENT,
    IdProduto INT NOT NULL,
    Quantidade INT NOT NULL,
    NumeroNota VARCHAR(30) NOT NULL,
    Fornecedor VARCHAR(120) NOT NULL DEFAULT '',
    Validade DATETIME(6) NULL,                -- opcional: nem todo produto vence
    CustoUnitario DECIMAL(10,2) NOT NULL,
    PrecoVendaAplicado DECIMAL(10,2) NOT NULL,
    DataEntrada DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    IdFuncionario INT NOT NULL,
    CONSTRAINT PK_entradas_estoque PRIMARY KEY (Id),
    CONSTRAINT FK_entradas_produto FOREIGN KEY (IdProduto) REFERENCES produtos(Id),
    CONSTRAINT FK_entradas_funcionario FOREIGN KEY (IdFuncionario) REFERENCES funcionarios(Id),
    CONSTRAINT CK_entradas_quantidade CHECK (Quantidade > 0),
    CONSTRAINT CK_entradas_valores CHECK (CustoUnitario > 0 AND PrecoVendaAplicado >= CustoUnitario),
    INDEX idx_entradas_produto (IdProduto, Validade),
    INDEX idx_entradas_data (DataEntrada)
) ENGINE=InnoDB;

-- Fornecedores cadastrados (a entrada de mercadoria guarda o nome do fornecedor)
CREATE TABLE IF NOT EXISTS fornecedores (
    Id INT NOT NULL AUTO_INCREMENT,
    Nome VARCHAR(120) NOT NULL,
    Cnpj VARCHAR(18) NOT NULL DEFAULT '',
    Telefone VARCHAR(20) NOT NULL DEFAULT '',
    Email VARCHAR(120) NOT NULL DEFAULT '',
    DataCadastro DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    Ativo TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT PK_fornecedores PRIMARY KEY (Id),
    CONSTRAINT UQ_fornecedores_nome UNIQUE (Nome)
) ENGINE=InnoDB;

-- Compatibilidade: bancos criados por versões anteriores tinham estas duas colunas como
-- NOT NULL. Em um banco novo os comandos abaixo não alteram nada.
ALTER TABLE vendas MODIFY IdTutor INT NULL;
ALTER TABLE historicos MODIFY IdAgendamento INT NULL;

-- Atualização de bancos já existentes: o MySQL não tem "ADD COLUMN IF NOT EXISTS",
-- então estas rotinas consultam o information_schema antes de alterar a tabela.
DROP PROCEDURE IF EXISTS sp_garantir_coluna;
DROP PROCEDURE IF EXISTS sp_recriar_check;

DELIMITER $$

CREATE PROCEDURE sp_garantir_coluna(IN p_tabela VARCHAR(64), IN p_coluna VARCHAR(64), IN p_definicao VARCHAR(255))
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.COLUMNS
                   WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = p_tabela AND COLUMN_NAME = p_coluna) THEN
        SET @ddl = CONCAT('ALTER TABLE ', p_tabela, ' ADD COLUMN ', p_coluna, ' ', p_definicao);
        PREPARE comando FROM @ddl;
        EXECUTE comando;
        DEALLOCATE PREPARE comando;
    END IF;
END$$

CREATE PROCEDURE sp_recriar_check(IN p_tabela VARCHAR(64), IN p_nome VARCHAR(64), IN p_expressao VARCHAR(255))
BEGIN
    IF EXISTS (SELECT 1 FROM information_schema.TABLE_CONSTRAINTS
               WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = p_tabela
                 AND CONSTRAINT_NAME = p_nome AND CONSTRAINT_TYPE = 'CHECK') THEN
        SET @ddl = CONCAT('ALTER TABLE ', p_tabela, ' DROP CHECK ', p_nome);
        PREPARE comando FROM @ddl;
        EXECUTE comando;
        DEALLOCATE PREPARE comando;
    END IF;
    SET @ddl = CONCAT('ALTER TABLE ', p_tabela, ' ADD CONSTRAINT ', p_nome, ' CHECK (', p_expressao, ')');
    PREPARE comando FROM @ddl;
    EXECUTE comando;
    DEALLOCATE PREPARE comando;
END$$

DELIMITER ;

CALL sp_garantir_coluna('agendamentos', 'ObservacoesAtendimento', 'TEXT NULL');
CALL sp_garantir_coluna('agendamentos', 'DataChegada', 'DATETIME(6) NULL');
CALL sp_garantir_coluna('agendamentos', 'ValorPago', 'DECIMAL(10,2) NULL');
CALL sp_garantir_coluna('agendamentos', 'FormaPagamento', 'INT NULL');
CALL sp_garantir_coluna('agendamentos', 'DataPagamento', 'DATETIME(6) NULL');
CALL sp_recriar_check('funcionarios', 'CK_funcionarios_cargo', 'Cargo BETWEEN 0 AND 3');

-- ============================================================
-- 3. TRIGGERS
-- ============================================================
DROP TRIGGER IF EXISTS trg_agendamento_calcula_fim;
DROP TRIGGER IF EXISTS trg_item_venda_baixa_estoque;
DROP TRIGGER IF EXISTS trg_item_venda_atualiza_total;
DROP TRIGGER IF EXISTS trg_agendamento_log_status;
DROP TRIGGER IF EXISTS trg_estoque_resolve_alerta;

DELIMITER $$

-- RN10: o fim do agendamento é sempre o início somado à duração do serviço
CREATE TRIGGER trg_agendamento_calcula_fim
BEFORE INSERT ON agendamentos
FOR EACH ROW
BEGIN
    DECLARE v_duracao INT;
    SELECT DuracaoMinutos INTO v_duracao
    FROM servicos WHERE Id = NEW.IdServico;
    IF v_duracao IS NOT NULL THEN
        SET NEW.DataHoraFim = DATE_ADD(NEW.DataHoraInicio, INTERVAL v_duracao MINUTE);
    END IF;
END$$

-- RN15: toda venda diminui o estoque e gera alerta no ponto de reposição
CREATE TRIGGER trg_item_venda_baixa_estoque
AFTER INSERT ON itensvenda
FOR EACH ROW
BEGIN
    UPDATE estoques
    SET QuantidadeAtual = QuantidadeAtual - NEW.Quantidade,
        UltimaAtualizacao = CURRENT_TIMESTAMP(6)
    WHERE IdProduto = NEW.IdProduto;

    -- Se atingiu o ponto de reposição, dispara o alerta
    INSERT INTO alerta_reposicao (IdProduto, QuantidadeAtual, PontoReposicao)
    SELECT IdProduto, QuantidadeAtual, PontoReposicao
    FROM estoques
    WHERE IdProduto = NEW.IdProduto
      AND QuantidadeAtual <= PontoReposicao;
END$$

-- Recalcula o total da venda automaticamente quando um item é inserido
CREATE TRIGGER trg_item_venda_atualiza_total
AFTER INSERT ON itensvenda
FOR EACH ROW
BEGIN
    UPDATE vendas
    SET Total = (SELECT IFNULL(SUM(Subtotal), 0) FROM itensvenda WHERE IdVenda = NEW.IdVenda)
    WHERE Id = NEW.IdVenda;
END$$

-- RN04: registra toda mudança de status do agendamento (auditoria)
CREATE TRIGGER trg_agendamento_log_status
AFTER UPDATE ON agendamentos
FOR EACH ROW
BEGIN
    IF OLD.Status <> NEW.Status THEN
        INSERT INTO log_agendamento_status (IdAgendamento, StatusAnterior, StatusNovo)
        VALUES (NEW.Id, OLD.Status, NEW.Status);
    END IF;
END$$

-- RN15: quando a reposição devolve o saldo acima do ponto, os alertas abertos são encerrados
CREATE TRIGGER trg_estoque_resolve_alerta
AFTER UPDATE ON estoques
FOR EACH ROW
BEGIN
    IF NEW.QuantidadeAtual > NEW.PontoReposicao AND OLD.QuantidadeAtual <= OLD.PontoReposicao THEN
        UPDATE alerta_reposicao
        SET Resolvido = 1
        WHERE IdProduto = NEW.IdProduto AND Resolvido = 0;
    END IF;
END$$

DELIMITER ;

-- ============================================================
-- 4. PROCEDURES
-- ============================================================
DROP PROCEDURE IF EXISTS sp_agendar_servico;
DROP PROCEDURE IF EXISTS sp_horarios_disponiveis;
DROP PROCEDURE IF EXISTS sp_registrar_venda;
DROP PROCEDURE IF EXISTS sp_receita_mensal;
DROP PROCEDURE IF EXISTS sp_estoque_critico;

DELIMITER $$

-- Procedure 1: agenda um serviço validando data futura, expediente (RN03) e conflito (RN01)
CREATE PROCEDURE sp_agendar_servico(
    IN p_id_pet INT,
    IN p_id_servico INT,
    IN p_id_funcionario INT,
    IN p_data_hora_inicio DATETIME,
    IN p_observacoes TEXT,
    OUT p_id_agendamento INT,
    OUT p_mensagem VARCHAR(255)
)
BEGIN
    DECLARE v_duracao INT;
    DECLARE v_data_hora_fim DATETIME;
    DECLARE v_conflito INT DEFAULT 0;

    SET p_id_agendamento = 0;

    -- RN10: duração do serviço
    SELECT DuracaoMinutos INTO v_duracao
    FROM servicos WHERE Id = p_id_servico AND Ativo = 1;

    SET v_data_hora_fim = DATE_ADD(p_data_hora_inicio, INTERVAL v_duracao MINUTE);

    IF v_duracao IS NULL THEN
        SET p_mensagem = 'Serviço não encontrado.';
    ELSEIF p_data_hora_inicio <= NOW() THEN
        SET p_mensagem = 'A data do agendamento deve ser futura.';
    ELSEIF DAYOFWEEK(p_data_hora_inicio) = 1 THEN          -- 1 = domingo
        SET p_mensagem = 'Fechado aos domingos (RN03).';
    ELSEIF TIME(p_data_hora_inicio) < '08:00:00'
        OR TIME(v_data_hora_fim) > '19:00:00'
        OR DATE(v_data_hora_fim) <> DATE(p_data_hora_inicio) THEN
        SET p_mensagem = 'Fora do horário de funcionamento, das 08h às 19h (RN03).';
    ELSE
        -- RN01: verifica sobreposição com o mesmo profissional
        SELECT COUNT(*) INTO v_conflito
        FROM agendamentos
        WHERE IdFuncionario = p_id_funcionario
          AND Status <> 3                                  -- ignora os cancelados
          AND p_data_hora_inicio < DataHoraFim
          AND v_data_hora_fim > DataHoraInicio;

        IF v_conflito > 0 THEN
            SET p_mensagem = 'Conflito de horário com outro atendimento (RN01).';
        ELSE
            INSERT INTO agendamentos
                (IdPet, IdServico, IdFuncionario, DataHoraInicio, DataHoraFim, Status, Observacoes, DataCriacao)
            VALUES
                (p_id_pet, p_id_servico, p_id_funcionario, p_data_hora_inicio, v_data_hora_fim, 0, p_observacoes, CURRENT_TIMESTAMP(6));
            SET p_id_agendamento = LAST_INSERT_ID();
            SET p_mensagem = 'Agendamento criado com sucesso.';
        END IF;
    END IF;
END$$

-- Procedure 2: retorna os horários livres de um profissional em uma data (intervalos de 15 minutos)
CREATE PROCEDURE sp_horarios_disponiveis(
    IN p_data DATE,
    IN p_id_servico INT,
    IN p_id_funcionario INT
)
BEGIN
    DECLARE v_duracao INT;
    DECLARE v_inicio DATETIME;
    DECLARE v_fechamento DATETIME;

    SELECT DuracaoMinutos INTO v_duracao FROM servicos WHERE Id = p_id_servico;

    DROP TEMPORARY TABLE IF EXISTS horarios_disponiveis_temp;
    CREATE TEMPORARY TABLE horarios_disponiveis_temp (data_hora DATETIME);

    SET v_inicio = TIMESTAMP(p_data, '08:00:00');          -- Abertura (RN03: das 08h às 19h)
    SET v_fechamento = TIMESTAMP(p_data, '19:00:00');

    -- Sem expediente aos domingos; o atendimento precisa terminar até o fechamento
    WHILE v_duracao IS NOT NULL
      AND DAYOFWEEK(p_data) <> 1
      AND DATE_ADD(v_inicio, INTERVAL v_duracao MINUTE) <= v_fechamento DO
        IF v_inicio > NOW() AND NOT EXISTS (
            SELECT 1 FROM agendamentos
            WHERE IdFuncionario = p_id_funcionario
              AND Status <> 3
              AND v_inicio < DataHoraFim
              AND DATE_ADD(v_inicio, INTERVAL v_duracao MINUTE) > DataHoraInicio
        ) THEN
            INSERT INTO horarios_disponiveis_temp (data_hora) VALUES (v_inicio);
        END IF;
        SET v_inicio = DATE_ADD(v_inicio, INTERVAL 15 MINUTE);
    END WHILE;

    SELECT data_hora FROM horarios_disponiveis_temp ORDER BY data_hora;
    DROP TEMPORARY TABLE IF EXISTS horarios_disponiveis_temp;
END$$

-- Procedure 3: registra uma venda completa em uma única transação.
-- p_itens é um JSON no formato [{"idProduto": 1, "quantidade": 2}, ...].
-- A baixa de estoque e o total são feitos pelas triggers de itensvenda; se faltar
-- saldo para qualquer item, a venda inteira é desfeita (ROLLBACK).
CREATE PROCEDURE sp_registrar_venda(
    IN p_id_tutor INT,
    IN p_id_funcionario INT,
    IN p_forma_pagamento INT,
    IN p_observacoes TEXT,
    IN p_itens JSON,
    OUT p_id_venda INT,
    OUT p_mensagem VARCHAR(255)
)
BEGIN
    DECLARE v_sem_saldo INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SET p_id_venda = 0;
        SET p_mensagem = 'Venda não registrada: dados inválidos ou estoque insuficiente.';
    END;

    SET p_id_venda = 0;

    START TRANSACTION;

    -- Itens pedidos acima do saldo disponível (ou sem registro de estoque)
    SELECT COUNT(*) INTO v_sem_saldo
    FROM JSON_TABLE(p_itens, '$[*]' COLUMNS (
            idProduto INT PATH '$.idProduto',
            quantidade INT PATH '$.quantidade')) AS i
    LEFT JOIN estoques e ON e.IdProduto = i.idProduto
    WHERE e.Id IS NULL OR e.QuantidadeAtual < i.quantidade OR i.quantidade <= 0;

    IF v_sem_saldo > 0 OR JSON_LENGTH(p_itens) = 0 THEN
        ROLLBACK;
        SET p_mensagem = 'Venda não registrada: estoque insuficiente para um ou mais itens.';
    ELSE
        INSERT INTO vendas (IdTutor, IdFuncionario, FormaPagamento, Observacoes, Total, DataVenda)
        VALUES (p_id_tutor, p_id_funcionario, p_forma_pagamento, p_observacoes, 0, CURRENT_TIMESTAMP(6));
        SET p_id_venda = LAST_INSERT_ID();

        INSERT INTO itensvenda (IdVenda, IdProduto, Quantidade, PrecoUnitario, Subtotal)
        SELECT p_id_venda, p.Id, i.quantidade, p.PrecoVenda, p.PrecoVenda * i.quantidade
        FROM JSON_TABLE(p_itens, '$[*]' COLUMNS (
                idProduto INT PATH '$.idProduto',
                quantidade INT PATH '$.quantidade')) AS i
        INNER JOIN produtos p ON p.Id = i.idProduto;

        COMMIT;
        SET p_mensagem = 'Venda registrada com sucesso.';
    END IF;
END$$

-- Procedure 4: relatório de receita mensal (produtos vendidos + serviços concluídos)
CREATE PROCEDURE sp_receita_mensal(
    IN p_ano INT,
    IN p_mes INT
)
BEGIN
    DECLARE v_inicio DATETIME DEFAULT STR_TO_DATE(CONCAT(p_ano, '-', p_mes, '-01'), '%Y-%m-%d');
    DECLARE v_fim DATETIME DEFAULT DATE_ADD(v_inicio, INTERVAL 1 MONTH);

    SELECT
        DATE_FORMAT(v_inicio, '%Y-%m') AS competencia,
        (SELECT COUNT(*) FROM vendas WHERE DataVenda >= v_inicio AND DataVenda < v_fim) AS total_vendas,
        (SELECT IFNULL(SUM(Total), 0) FROM vendas WHERE DataVenda >= v_inicio AND DataVenda < v_fim) AS receita_produtos,
        (SELECT IFNULL(AVG(Total), 0) FROM vendas WHERE DataVenda >= v_inicio AND DataVenda < v_fim) AS ticket_medio,
        (SELECT IFNULL(SUM(IFNULL(a.ValorPago, s.Preco)), 0)   -- valor recebido no balcão, ou o preço de tabela
           FROM agendamentos a INNER JOIN servicos s ON s.Id = a.IdServico
          WHERE a.Status = 2 AND a.DataHoraInicio >= v_inicio AND a.DataHoraInicio < v_fim) AS receita_servicos;
END$$

-- Procedure 5: produtos em estoque crítico (ponto de reposição)
CREATE PROCEDURE sp_estoque_critico()
BEGIN
    SELECT Id, Nome, QuantidadeAtual, PontoReposicao, SaldoAbaixoPonto
    FROM vw_estoque_critico
    ORDER BY QuantidadeAtual ASC;
END$$

DELIMITER ;

-- ============================================================
-- 5. VIEWS
-- ============================================================

-- Visão de estoque crítico para alertas (RN15)
CREATE OR REPLACE VIEW vw_estoque_critico AS
SELECT
    p.Id,
    p.Nome,
    e.QuantidadeAtual,
    e.PontoReposicao,
    (e.QuantidadeAtual - e.PontoReposicao) AS SaldoAbaixoPonto
FROM estoques e
INNER JOIN produtos p ON p.Id = e.IdProduto
WHERE e.QuantidadeAtual <= e.PontoReposicao;

-- Visão de receita por serviço (apoia relatórios gerenciais)
CREATE OR REPLACE VIEW vw_receita_por_servico AS
SELECT
    s.Id,
    s.Nome AS Servico,
    COUNT(a.Id) AS QtdAtendimentos,
    IFNULL(SUM(IF(a.Id IS NULL, 0, s.Preco)), 0) AS ReceitaEstimada
FROM servicos s
LEFT JOIN agendamentos a ON a.IdServico = s.Id AND a.Status IN (1, 2) -- CONFIRMADO/CONCLUIDO
GROUP BY s.Id, s.Nome;

-- Visão da agenda com os nomes resolvidos (pet, tutor, serviço e profissional)
CREATE OR REPLACE VIEW vw_agenda_detalhada AS
SELECT
    a.Id,
    a.DataHoraInicio,
    a.DataHoraFim,
    a.Status,
    p.Nome AS Pet,
    t.Nome AS Tutor,
    s.Nome AS Servico,
    f.Nome AS Profissional
FROM agendamentos a
INNER JOIN pets p ON p.Id = a.IdPet
INNER JOIN tutores t ON t.Id = p.TutorId
INNER JOIN servicos s ON s.Id = a.IdServico
INNER JOIN funcionarios f ON f.Id = a.IdFuncionario;
