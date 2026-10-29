from contextlib import asynccontextmanager
from pathlib import Path

import joblib
import numpy as np
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field
from sklearn.cluster import KMeans
from sklearn.ensemble import RandomForestClassifier
from sklearn.linear_model import LinearRegression
from sklearn.model_selection import train_test_split
from sklearn.pipeline import make_pipeline
from sklearn.preprocessing import StandardScaler

# Os modelos ficam ao lado deste arquivo, qualquer que seja a pasta de onde o uvicorn é iniciado
MODEL_PATH = Path(__file__).resolve().parent / "modelos_patafacil.joblib"
SEMENTE = 42

modelos = {}


# 1. Dados sintéticos de treino (Mitigação de Risco: o pet shop ainda não tem histórico real suficiente).
#    As regras abaixo reproduzem o comportamento esperado do negócio; quando houver dados reais no
#    banco, basta trocar estas funções pela leitura das tabelas de agendamentos e vendas.
def gerar_dados_cancelamento(rng, n=600):
    antecedencia = rng.integers(0, 31, n)
    historico = rng.integers(0, 5, n)
    servico = rng.integers(1, 4, n)        # 1-Banho, 2-Consulta, 3-Vacina
    dia_semana = rng.integers(1, 7, n)     # 0-Domingo ... 6-Sábado (padrão do C#); domingo fecha
    tutor_novo = rng.integers(0, 2, n)

    # Quem já cancelou tende a cancelar de novo; agendar com muita antecedência e ser novato também pesam
    logit = -2.2 + 0.9 * historico + 0.04 * antecedencia + 0.6 * tutor_novo + 0.3 * (dia_semana == 6)
    probabilidade = 1 / (1 + np.exp(-logit))
    cancelou = (rng.random(n) < probabilidade).astype(int)

    X = np.column_stack([antecedencia, historico, servico, dia_semana, tutor_novo])
    return X, cancelou


def gerar_dados_demanda(rng, n=400):
    dia_semana = rng.integers(1, 7, n)
    mes = rng.integers(1, 13, n)
    feriado = (rng.random(n) < 0.05).astype(int)
    promocao = (rng.random(n) < 0.20).astype(int)
    temperatura = rng.normal(24, 5, n)

    # Mais movimento perto do fim de semana, em promoções e em dias quentes; feriado derruba a agenda
    agendamentos = (8 + 2.0 * dia_semana + 6 * promocao - 5 * feriado
                    + 0.3 * (temperatura - 24) + rng.normal(0, 2, n))
    X = np.column_stack([dia_semana, mes, feriado, promocao, temperatura])
    return X, np.clip(agendamentos, 0, None)


def gerar_dados_segmentacao(rng, n_por_perfil=80):
    # Colunas: frequencia_anual, ticket_medio, recencia_dias, num_pets
    perfis = [
        ((2, 60, 90, 1), (1, 15, 25, 0.3)),     # Ocasionais
        ((3, 80, 240, 1), (1, 20, 40, 0.3)),    # Em risco (não aparecem há muito tempo)
        ((10, 120, 20, 2), (2, 25, 8, 0.6)),    # Fiéis
        ((22, 260, 7, 3), (4, 40, 4, 0.8)),     # VIP
    ]
    blocos = [rng.normal(media, desvio, (n_por_perfil, 4)) for media, desvio in perfis]
    return np.clip(np.vstack(blocos), 0, None)


def nomear_clusters(pipeline_segmentacao):
    # O K-Means numera os grupos de forma arbitrária: os nomes são atribuídos pelos centroides
    kmeans = pipeline_segmentacao.named_steps["kmeans"]
    centroides = pipeline_segmentacao.named_steps["standardscaler"].inverse_transform(kmeans.cluster_centers_)

    restantes = list(range(len(centroides)))
    nomes = {}

    def separar(nome, chave):
        escolhido = max(restantes, key=chave)
        nomes[escolhido] = nome
        restantes.remove(escolhido)

    separar("VIP", lambda i: centroides[i][0] * centroides[i][1])   # maior gasto anual
    separar("Em Risco", lambda i: centroides[i][2])                  # maior recência (sumidos)
    separar("Fiéis", lambda i: centroides[i][0])                     # mais frequentes entre os restantes
    for i in restantes:
        nomes[i] = "Ocasionais"
    return nomes


def treinar_modelos():
    rng = np.random.default_rng(SEMENTE)

    print("Treinando modelo de Risco de Cancelamento (Random Forest)...")
    X, y = gerar_dados_cancelamento(rng)
    X_treino, X_teste, y_treino, y_teste = train_test_split(X, y, test_size=0.25, random_state=SEMENTE, stratify=y)
    cancelamento = RandomForestClassifier(n_estimators=200, max_depth=6, random_state=SEMENTE)
    cancelamento.fit(X_treino, y_treino)
    acuracia = cancelamento.score(X_teste, y_teste)

    print("Treinando modelo de Demanda (Regressão Linear)...")
    X, y = gerar_dados_demanda(rng)
    X_treino, X_teste, y_treino, y_teste = train_test_split(X, y, test_size=0.25, random_state=SEMENTE)
    demanda = LinearRegression().fit(X_treino, y_treino)
    r2 = demanda.score(X_teste, y_teste)

    print("Treinando modelo de Segmentação (K-Means)...")
    segmentacao = make_pipeline(StandardScaler(), KMeans(n_clusters=4, random_state=SEMENTE, n_init=10))
    segmentacao.fit(gerar_dados_segmentacao(rng))

    return {
        "cancelamento": cancelamento,
        "demanda": demanda,
        "segmentacao": segmentacao,
        "perfis": nomear_clusters(segmentacao),
        "metricas": {"acuracia_cancelamento": round(float(acuracia), 3), "r2_demanda": round(float(r2), 3)},
    }


# 2. Evento de Inicialização: carrega os modelos do disco ou treina e serializa novos
@asynccontextmanager
async def inicializar_modelos(app: FastAPI):
    if MODEL_PATH.exists():
        modelos.update(joblib.load(MODEL_PATH))
        print("Modelos carregados com sucesso do disco.")
    else:
        modelos.update(treinar_modelos())
        joblib.dump(modelos, MODEL_PATH)
        print("Modelos treinados e salvos!")
    print(f"🤖 Todos os 3 modelos de Inteligência Artificial prontos! Métricas: {modelos['metricas']}")
    yield


app = FastAPI(title="PataFácil - API de Machine Learning", version="2.0", lifespan=inicializar_modelos)


def obter_modelo(nome):
    if nome not in modelos:
        raise HTTPException(status_code=503, detail="Modelo de Machine Learning indisponível.")
    return modelos[nome]


@app.get("/health")
def health():
    return {"status": "ok", "modelos": sorted(k for k in modelos if k not in ("perfis", "metricas")), "metricas": modelos.get("metricas")}


# 3. Rota de Previsão de Risco — tipagem Pydantic valida a entrada
class AgendamentoFeatures(BaseModel):
    antecedencia_dias: int = Field(ge=0)
    historico_cancelamentos: int = Field(ge=0)
    tipo_servico: int                              # Ex: 1-Banho, 2-Consulta, 3-Vacina
    dia_semana: int = Field(ge=0, le=6)            # 0-Domingo ... 6-Sábado (DayOfWeek do C#)
    tutor_novo: int = Field(ge=0, le=1)            # 1-Sim (Novo), 0-Não (Veterano)


@app.post("/api/ml/prever-cancelamento")
def prever_cancelamento(features: AgendamentoFeatures):
    dados_entrada = [[
        features.antecedencia_dias, features.historico_cancelamentos,
        features.tipo_servico, features.dia_semana, features.tutor_novo
    ]]

    # Extrai a probabilidade da classe 1 (Cancelamento)
    probabilidade = float(obter_modelo("cancelamento").predict_proba(dados_entrada)[0][1])
    risco = "ALTO" if probabilidade > 0.5 else "BAIXO"

    return {"risco": risco, "probabilidade": round(probabilidade, 2)}


class DemandaFeatures(BaseModel):
    dia_semana: int = Field(ge=0, le=6)
    mes: int = Field(ge=1, le=12)
    feriado: int = Field(ge=0, le=1)
    promocao: int = Field(ge=0, le=1)
    temperatura: float


@app.post("/api/ml/prever-demanda")
def prever_demanda(features: DemandaFeatures):
    # Domingo não tem expediente (RN03): a demanda é zero por regra, não por previsão
    if features.dia_semana == 0:
        return {"previsao_agendamentos": 0}

    dados = [[features.dia_semana, features.mes, features.feriado, features.promocao, features.temperatura]]
    previsao = float(obter_modelo("demanda").predict(dados)[0])
    return {"previsao_agendamentos": max(0, round(previsao))}


class TutorFeatures(BaseModel):
    frequencia_anual: int = Field(ge=0)
    ticket_medio: float = Field(ge=0)
    recencia_dias: int = Field(ge=0)
    num_pets: int = Field(ge=0)


@app.post("/api/ml/segmentar-tutor")
def segmentar_tutor(features: TutorFeatures):
    dados = [[features.frequencia_anual, features.ticket_medio, features.recencia_dias, features.num_pets]]
    cluster = int(obter_modelo("segmentacao").predict(dados)[0])
    return {"cluster": cluster, "perfil": obter_modelo("perfis").get(cluster, "Indefinido")}
