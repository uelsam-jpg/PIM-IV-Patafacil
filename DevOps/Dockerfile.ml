# ============================================================
# Dockerfile — API de Machine Learning (Python/FastAPI)
# Contexto de build: raiz do projeto (contém a pasta MachineLearning/)
#   docker build -f DevOps/Dockerfile.ml -t patafacil-ml .
# ============================================================

FROM python:3.13-slim

WORKDIR /app

# Instala as dependências primeiro (cache de camadas)
COPY MachineLearning/requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

# Copia o código da IA
COPY MachineLearning/ ./

EXPOSE 8000

# uvicorn exposto para todas as interfaces (necessário dentro do Docker)
CMD ["uvicorn", "ml_api:app", "--host", "0.0.0.0", "--port", "8000"]
