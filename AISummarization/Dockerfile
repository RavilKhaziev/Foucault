FROM python:3.12.4-slim AS base

WORKDIR /app
RUN mkdir -p /app/req
RUN ls
RUN pwd
COPY ./requirements.txt /app/req
RUN ls
RUN pwd
RUN pip install -r /app/req/requirements.txt

FROM base AS finale
WORKDIR /app
COPY . .
EXPOSE 8081
CMD ["uvicorn", "server:app", "--host", "0.0.0.0", "--port", "8081"]