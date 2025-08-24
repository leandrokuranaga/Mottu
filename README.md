# Como executar o projeto

## Via Docker (recomendado)

1. Vá para a pasta **raiz** do projeto.
2. Suba os serviços com:

```bash
docker-compose up -d
```

3. Você verá as instâncias inicializadas no app do Docker:

![Instâncias no Docker](https://github.com/user-attachments/assets/f93fb8f0-0da5-42a8-936e-568c876b02ce)

---

## Manualmente (sem Docker)

* Configure os serviços conforme seu `appsettings.development.json`.
* Inicie os serviços necessários e **start** a aplicação manualmente.

---

[Wiki](https://github.com/leandrokuranaga/Mottu/wiki)
[Board](https://github.com/users/leandrokuranaga/projects/23)


## Build & Tests
| CI | Status |
| --- | --- | 
| Validação PR| [![Pré-validação de Release/Hotfix](https://github.com/leandrokuranaga/mottu/actions/workflows/validate-pr.yml/badge.svg)](https://github.com/leandrokuranaga/mottu/actions/workflows/validate-pr.yml)
