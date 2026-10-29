// Controle de animação dos formulários (Baseado no seu CSS)

function mostrarCadastro() {
    const loginForm = document.getElementById("login-form");
    const cadastroForm = document.getElementById("cadastro-form");
    const btnBackground = document.getElementById("btn-background");

    if (loginForm) loginForm.style.left = "-150%";
    if (cadastroForm) cadastroForm.style.left = "50%";
    if (btnBackground) btnBackground.style.left = "110px";
    marcarAbaLogin(1);
}

// Deixa legível (texto branco) o botão que está sobre o fundo laranja
function marcarAbaLogin(indice) {
    document.querySelectorAll(".toggle-btn").forEach((botao, i) => botao.classList.toggle("ativo", i === indice));
}

function mostrarLogin() {
    const loginForm = document.getElementById("login-form");
    const cadastroForm = document.getElementById("cadastro-form");
    const btnBackground = document.getElementById("btn-background");

    if (loginForm) loginForm.style.left = "50%";
    if (cadastroForm) cadastroForm.style.left = "150%";
    if (btnBackground) btnBackground.style.left = "0";
    marcarAbaLogin(0);
}

// =========================================
// INTEGRAÇÃO COM A API C# (TutoresController)
// =========================================

// ATENÇÃO: Verifique qual porta o seu Swagger abre e troque o '5123' abaixo se necessário
const API_URL = "http://localhost:5288/api";

// Função blindada para extrair o erro exato que o C# enviou
async function extrairErroAPI(response) {
    try {
        const data = await response.json();
        if (data.mensagem) return data.mensagem;
        if (data.Erro || data.erro) return data.Erro || data.erro;
        // Pega os erros automáticos do ASP.NET (ex: faltou preencher algo)
        if (data.errors) return Object.values(data.errors).flat().join('\n'); 
        if (data.title) return data.title;
        return "Erro desconhecido retornado pelo servidor.";
    } catch (e) {
        return `Erro de comunicação (Status: ${response.status}).`;
    }
}

async function cadastrarTutor(event) {
    if (event) event.preventDefault(); // Evita recarregar a página se o event não for passado

    const cpfDigitado = document.getElementById("cad-cpf")?.value || "";

    // Validação básica para impedir textos como "teste" no lugar do CPF
    if (cpfDigitado.replace(/\D/g, "").length !== 11) {
        alert("Por favor, digite um CPF válido, com 11 números.");
        return;
    }

    // O balcão usa o telefone para falar com o cliente: DDD + número
    const telefoneDigitado = document.getElementById("cad-telefone")?.value || "";
    const digitosTelefone = telefoneDigitado.replace(/\D/g, "");
    if (digitosTelefone.length < 10 || digitosTelefone.length > 11) {
        alert("Por favor, digite o telefone com DDD. Ex.: (11) 98765-4321");
        return;
    }

    // Coleta os dados digitados
    const tutor = {
        nome: document.getElementById("cad-nome")?.value || "",
        cpf: cpfDigitado,
        email: document.getElementById("cad-email")?.value.trim() || "",
        senhaHash: document.getElementById("cad-senha")?.value || "",
        telefone: digitosTelefone,
        // Campos obrigatórios do banco preenchidos com padrão por enquanto
        logradouro: "A preencher",
        numero: "S/N",
        bairro: "A preencher",
        cidade: "A preencher",
        uf: "SP",
        cep: "00000-000"
    };

    try {
        const response = await fetch(`${API_URL}/tutores`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(tutor)
        });

        if (response.ok) {
            alert("Cadastro realizado com sucesso! Faça login para entrar.");
            mostrarLogin();
        } else {
            const erro = await extrairErroAPI(response);
            alert("Não foi possível cadastrar:\n" + erro);
        }
    } catch (error) {
        console.error("Erro:", error);
        alert("Falha grave na rede. Verifique se a API C# está rodando na porta correta (5288).");
    }
}

// =========================================
// FUNÇÃO DE LOGIN E AUTENTICAÇÃO (JWT)
// =========================================
async function realizarLogin(event) {
    if (event) event.preventDefault();

    const credenciais = {
        email: document.getElementById("log-email")?.value.trim() || "",
        senha: document.getElementById("log-senha")?.value || ""
    };

    try {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(credenciais)
        });

        if (response.ok) {
            const data = await response.json();
            
            // PADRONIZA O PERFIL (Evita que o C# envie "0" ou "Veterinário" com acento e quebre o JS)
            const pStr = String(data.usuario.perfil).toLowerCase();
            if (pStr.includes("vet") || pStr === "0") data.usuario.perfil = "Veterinario";
            else if (pStr.includes("aten") || pStr === "1") data.usuario.perfil = "Atendente";
            else if (pStr.includes("geren") || pStr === "2") data.usuario.perfil = "Gerente";
            else if (pStr.includes("tosa") || pStr === "3") data.usuario.perfil = "Tosador";
            else data.usuario.perfil = "Tutor";

            // Salva o Token de segurança e os dados do usuário no navegador
            localStorage.setItem("token", data.token);
            localStorage.setItem("usuario", JSON.stringify(data.usuario));
            
            alert(`Bem-vindo(a), ${data.usuario.nome}! Redirecionando...`);
            
            // Redireciona para o painel correto de acordo com o perfil
            if (data.usuario.perfil === "Tutor") {
                window.location.href = "portal.html";
            } else if (data.usuario.perfil === "Veterinario") {
                window.location.href = "veterinario.html";
            } else if (data.usuario.perfil === "Atendente" || data.usuario.perfil === "Tosador") {
                window.location.href = "atendente.html"; // O balcão abre direto na agenda
            } else {
                window.location.href = "dashboard.html"; // Gerente vai para a Visão Geral
            }
        } else {
            const erro = await extrairErroAPI(response);
            alert("Acesso negado:\n" + erro);
        }
    } catch (error) {
        console.error("Erro no login:", error);
        alert("Erro ao conectar com o servidor.");
    }
}