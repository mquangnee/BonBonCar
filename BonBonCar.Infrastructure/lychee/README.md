## Lychee (local Docker)

### Run

From this folder (`BonBonCar.Infrastructure/lychee`):

1) Copy `.env.example` → `.env` and adjust values if needed
2) Start containers:

```bash
docker compose up -d
```

3) Open Lychee at `http://localhost:8000` (or the port you set in `LYCHEE_PORT`)

### Notes

- This compose file uses **no host bind-mounts**. Uploads + database live inside the containers.
- If you remove/recreate containers (e.g. `docker compose down` then `up`), you will lose uploads and DB data.
- `.env` is ignored by git (keep secrets out of the repo).
