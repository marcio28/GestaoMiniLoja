# Feedback - Avaliação Geral

## Front End
### Navegação
  * Pontos positivos:
    - O projeto possui views e rotas definidas para as funcionalidades no projeto MVC.
    - Implementação de controllers e views para operações principais.

  * Pontos negativos:
    - Nomenclatura verbosa e fora do padrão nas actions das controllers.
    - Nomes de métodos e classes excessivamente descritivos, prejudicando a legibilidade.

### Design
    - Será avaliado na entrega final

### Funcionalidade
  * Pontos positivos:
    - Implementações básicas de CRUD no MVC funcionando.
    - Migrations e seed de dados automáticos implementados.

  * Pontos negativos:
    - Nomenclatura não segue padrões convencionais do ASP.NET Core.

## Back End
### Arquitetura
  * Pontos positivos:
    - Separação em camadas básica presente.
    - Implementação do SQLite conforme especificado.

  * Pontos negativos:
    - Ausência da camada de API RESTful.
    - A camada "Data" possui responsabilidades além de acesso a dados, deveria ser renomeada para "Core".
    - Nomenclatura verbosa e inconsistente com padrões de mercado.

### Funcionalidade
  * Pontos positivos:
    - Uso do Entity Framework Core com SQLite.
    - Implementação de migrations automáticas.
    - Seed de dados automático funcionando.

  * Pontos negativos:
    - Ausência da API RESTful.
    - Nomenclatura dos métodos e classes muito verbosa.
    - Falta a implementação da criação automática do vendedor durante o registro do Identity.

### Modelagem
  * Pontos positivos:
    - Modelagem de entidades presente e funcional.
    - Uso adequado do Entity Framework Core.

  * Pontos negativos:
    - Camada "Data" com responsabilidades além do acesso a dados.
    - Nomenclatura das classes e métodos necessita revisão para seguir padrões mais concisos.

## Projeto
### Organização
  * Pontos positivos:
    - Uso da pasta `src` na raiz.
    - Arquivo de solução (`.sln`) presente.
    - Estrutura básica de projetos estabelecida.

  * Pontos negativos:
    - Falta do projeto de API.

### Documentação
  * Pontos positivos:
    - Repositório com `README.md` presente.
    - Arquivo `FEEDBACK.md` presente.

  * Pontos negativos:
    - Não há documentação da API (pois ela não existe).

### Instalação
  * Pontos positivos:
    - Implementação correta do SQLite.
    - Migrations automáticas funcionando.
    - Seed de dados implementado.
