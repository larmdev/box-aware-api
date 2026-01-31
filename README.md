# box-aware-api

### endpoint url
```
https://box-aware-api.onrender.com
```

### ขั้นตอนการ รัน project บน localhost
```
cd Box

dotnet run --project Box.API

endpoint: http://localhost:5028
```

### ขั้นตอนการ รัน project บน docker
```
[root dir]

docker compose up --build

endpoint: http://localhost:8080
```
### Auth เพื่อเอา access_token ไปใช้งาน
```
POST https://box-aware-api.onrender.com/api/auth/login
```

### Example 1
```
GET https://box-aware-api.onrender.com/api/students
GET https://box-aware-api.onrender.com/api/students?offset=0&limit=10
```

### Example 2
```
POST https://box-aware-api.onrender.com/api/rank

request body:

{
	"p1": "A,B,1,2,1,AA,3,5,BB,4,2,4,AA,B"
}
```
### Example 3
```
GET https://box-aware-api.onrender.com/api/todo
GET https://box-aware-api.onrender.com/api/todo/88
```
### Unit Test
```
cd Box
dotnet test
```

### Docker postgres
```
docker run -d \
  --name postgres-dev \
  -e POSTGRES_USER=appuser \
  -e POSTGRES_PASSWORD=apppass \
  -e POSTGRES_DB=appdb \
  -p 5432:5432 \
  -v pgdata:/var/lib/postgresql/data \
  postgres:16


"Host=localhost;Port=5432;Database=appdb;Username=appuser;Password=apppass"

```
```
// docker-compose.yml

version: "3.9"
services:
  postgres:
    image: postgres:16
    container_name: postgres-dev
    restart: always
    environment:
      POSTGRES_USER: appuser
      POSTGRES_PASSWORD: apppass
      POSTGRES_DB: appdb
    ports:
      - "5432:5432"
    volumes:
      - pgdata:/var/lib/postgresql/data

volumes:
  pgdata:

```
```
dotnet ef migrations add Init01 \
  -p Box/Box.Infrastructure/Box.Infrastructure.csproj \
  -s Box/Box.API/Box.API.csproj

dotnet ef database update \
  -p Box/Box.Infrastructure/Box.Infrastructure.csproj \
  -s Box/Box.API/Box.API.csproj


```
