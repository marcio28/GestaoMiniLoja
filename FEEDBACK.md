# Feedback - Avaliação Geral

## Front End

### Navegação
  * Pontos positivos:
    - O projeto MVC está bem estruturado, com navegação funcional, views completas e controllers para categorias e produtos.

  * Pontos negativos:
    - Nenhum.

### Design
  - Interface clara, funcional e bem adaptada para o uso administrativo de uma mini loja.

### Funcionalidade
  * Pontos positivos:
    - CRUD completo para produtos e categorias implementado tanto no MVC quanto na API.
    - Identity implementado corretamente com autenticação via cookies (MVC) e JWT (API).
    - A criação do vendedor é feita junto do usuário no MVC, com compartilhamento correto do ID.
    - Uso de SQLite com migrations automáticas e seed de dados bem configurado.
    - Arquitetura enxuta e coesa em três projetos: API, MVC e Core.
    - Modelagem das entidades atende aos requisitos do domínio.

  * Pontos negativos:
    - A criação do vendedor ocorre apenas no MVC, a API não realiza esse processo.
    - Falta verificação de segurança no endpoint de edição de produto para garantir que o vendedor logado seja o proprietário do item a ser alterado.

## Back End

### Arquitetura
  * Pontos positivos:
    - Três camadas bem definidas: Core, API e MVC.
    - Uso de boas práticas em configuração e separação de responsabilidades.

  * Pontos negativos:
    - Nenhum relevante além dos aspectos de segurança mencionados.

### Funcionalidade
  * Pontos positivos:
    - Implementações completas das operações CRUD e autenticação nas duas camadas.

  * Pontos negativos:
    - Validação de propriedade de recursos (vendedor-produto) ausente.

### Modelagem
  * Pontos positivos:
    - Modelagem clara, com associações corretas entre Produto, Categoria e Vendedor.
    - ViewModels e entidades bem definidas.

  * Pontos negativos:
    - Nenhum.

## Projeto

### Organização
  * Pontos positivos:
    - Organização geral do projeto clara e compatível com os padrões esperados.
    - Uso correto da pasta `src`, projeto `.sln` na raiz, e arquivos de documentação presentes.

  * Pontos negativos:
    - Nenhum.

### Documentação
  * Pontos positivos:
    - `README.md` e `FEEDBACK.md` presentes e com informações relevantes.
    - Swagger configurado na API.

  * Pontos negativos:
    - Nenhum.

### Instalação
  * Pontos positivos:
    - SQLite com seed e migrations automáticas funcionam corretamente.
    - Projeto executável localmente sem necessidade de ajustes manuais.

  * Pontos negativos:
    - Nenhum.

---

# 📊 Matriz de Avaliação de Projetos

| **Critério**                   | **Peso** | **Nota** | **Resultado Ponderado**                  |
|-------------------------------|----------|----------|------------------------------------------|
| **Funcionalidade**            | 30%      | 9        | 2,7                                      |
| **Qualidade do Código**       | 20%      | 8        | 1,6                                      |
| **Eficiência e Desempenho**   | 20%      | 8        | 1,6                                      |
| **Inovação e Diferenciais**   | 10%      | 9        | 0,9                                      |
| **Documentação e Organização**| 10%      | 9        | 0,9                                      |
| **Resolução de Feedbacks**    | 10%      | 8        | 0,8                                      |
| **Total**                     | 100%     | -        | **8,5**                                  |

## 🎯 **Nota Final: 8,5 / 10**
