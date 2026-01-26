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
