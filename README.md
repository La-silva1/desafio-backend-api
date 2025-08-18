# Desafio Backend - Sistema de Gerenciamento de Motos

Este é um projeto de API backend para gerenciamento de motos, entregadores e locações, desenvolvido em C# com .NET.

## Pré-requisitos

Antes de começar, certifique-se de que você tem os seguintes softwares instalados em sua máquina:

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Como Executar a Aplicação

Com o Docker e o Docker Compose instalados, siga os passos abaixo para executar a aplicação em seu ambiente local.

1.  **Clone o Repositório** (caso ainda não tenha feito):
    ```bash
    gh repo clone La-silva1/desafio-backend-api
    
2.  **Construa e Inicie os Contêineres:**
    Na pasta raiz do projeto, execute o seguinte comando:
    ```bash
    docker-compose up --build
    ```
    Este comando irá:
    - Construir a imagem Docker para a aplicação .NET.
    - Baixar a imagem oficial do PostgreSQL.
    - Iniciar ambos os contêineres.
    - A aplicação se encarregará de aplicar as migrations do Entity Framework automaticamente no banco de dados.

3.  **Acesse a Aplicação:**
    Após a inicialização, a API estará disponível em `http://localhost:8080`.

    Para visualizar e interagir com os endpoints disponíveis, acesse a documentação do Swagger UI no seu navegador:
    [http://localhost:8080/swagger](http://localhost:8080/swagger)

## Tecnologias Utilizadas

- **Backend:** C# com .NET 9
- **Banco de Dados:** PostgreSQL
- **ORM:** Entity Framework Core
- **Containerização:** Docker & Docker Compose
