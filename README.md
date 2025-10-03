# 🏆 Real-Time Leaderboard API 🏆

A high-performance, scalable **Real-Time Leaderboard System** built with **.NET 8** and **Redis Sorted Sets** for lightning-fast rankings and score management across multiple games and activities.
_This project is an implementation of the [Real-Time Leaderboard challenge](https://roadmap.sh/projects/realtime-leaderboard-system) from roadmap.sh._

---

## 🌟 **Project Overview**

This API provides a complete solution for managing competitive gaming leaderboards with real-time updates. Players can register, submit scores, and instantly see their rankings across global and game-specific leaderboards. Built with enterprise-grade architecture featuring JWT authentication, Redis caching, and comprehensive error handling.

### ✨ **Key Features**

- 🔐 **Secure Authentication** - JWT-based auth with refresh tokens
- 🚀 **Real-time Rankings** - Redis Sorted Sets for instant leaderboard updates
- 🎮 **Multi-Game Support** - Separate rankings for different games/activities
- 📊 **Historical Reports** - Top player analytics with date range filtering
- 🔄 **Token Management** - Automatic refresh and logout capabilities
- 📱 **RESTful API** - Clean, documented endpoints
- 🏗️ **Modern Architecture** - Clean separation of concerns with dependency injection

---

## 🛠️ **Technology Stack**

| Technology | Purpose | Version |
|------------|---------|---------|
| .NET 8 | Runtime & Framework | Latest |
| Entity Framework Core | Data Access Layer | 9.0.7 |
| SQL Server | Primary Database | Latest |
| Redis | Caching & Real-time Rankings | StackExchange.Redis |
| JWT Bearer | Authentication | 8.0.19 |
| Argon2 | Password Hashing | 1.3.1 |
| Swagger/OpenAPI | API Documentation | Automatic |

---

## 📋 **System Requirements**

### Required Software
- **.NET 8 SDK** or later
- **SQL Server** (LocalDB supported for development)
- **Redis Server** (Windows: Redis for Windows, Linux: Docker or native)

### Development Environment
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** for version control

---

## 🚀 **Quick Start Guide**

### 1. **Clone the Repository**
```bash
git clone https://github.com/mimescua/2508-real-time-leaderboard-api.git
cd 2508-real-time-leaderboard-api
```

### 2. **Configure Services**

#### **SQL Server Setup**
Ensure SQL Server is running and accessible. Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "LeaderBoardConnection": "Server=<URL>,<PORT>;Database=<DB>;User=<USER>;Password=<PASS>;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### **Redis Server Setup**

**Windows:**
1. Download Redis for Windows from [GitHub](https://github.com/microsoftarchive/redis/releases)
2. Run Redis Server
3. Verify connection on port 6379

**Docker:**
```bash
docker run -d -p 6379:6379 --name redis-server redis:latest
```

Update Redis configuration:
```json
{
  "ConnectionStrings": {
    "RedisConnection": "<URL>:<PORT>,user=<USER>,password=<PASS>"
  }
}
```

#### **JWT Configuration**
Set up your JWT secrets in `appsettings.json`:
```json
{
  "JWT": {
    "ValidAudience": "<AUDIENCE-URL>",
    "ValidIssuer": "<ISSUER-URL>",
    "Secret": "<SUPER-SECRET-KEY>"
  }
}
```

### 3. **Install Dependencies**
```bash
dotnet restore
```

### 4. **Run Database Migrations**
```bash
dotnet ef database update
```

### 5. **Launch the Application**
```bash
dotnet run
```

The API will be available at:
- **HTTPS**: `https://localhost:7xxx`
- **HTTP**: `http://localhost:5xxx`
- **Swagger UI**: `https://localhost:7xxx/swagger` (Development only)

---

## 📡 **API Endpoints**

### 🔐 **Authentication (`/Auth`)**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/Auth/Login` | User login | ❌ |
| `POST` | `/Auth/Logout` | User logout | ❌ |
| `POST` | `/Auth/RefreshToken` | Refresh access token | ❌ |

### 👥 **User Management (`/User`)**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/User/All` | Get all users | ✅ |
| `GET` | `/User/{id}` | Get user by ID | ✅ |
| `POST` | `/User/Create` | Create new user | ✅ |
| `PUT` | `/User/{id}` | Update user | ✅ |
| `DELETE` | `/User/{id}` | Delete user | ✅ |

### 🎮 **Game Management (`/Game`)**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/Game/All` | Get all games | ✅ |
| `GET` | `/Game/{id}` | Get game by ID | ✅ |
| `POST` | `/Game/Create` | Create new game | ✅ |
| `PUT` | `/Game/{id}` | Update game | ✅ |
| `DELETE` | `/Game/{id}` | Delete game | ✅ |

### 🏅 **Scoring System (`/Score`)**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `PUT` | `/Score/Submit/Game/{gameId}/User/{userId}` | Submit score | ✅ |

### 🎯 **Leaderboards (`/Leaderboard`)**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/Leaderboard/Global` | Global leaderboard (paginated) | ✅ |
| `GET` | `/Leaderboard/Game/{gameId}` | Game-specific leaderboard | ✅ |
| `GET` | `/Leaderboard/Rank/{userId}` | User's global ranking | ✅ |
| `GET` | `/Leaderboard/Game/{gameId}/Rank/{userId}` | User's game-specific ranking | ✅ |

### 📈 **Historical Reports (`/Historical`)**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/Historical/TopPlayers/Global` | Global top players report | ✅ |
| `GET` | `/Historical/TopPlayers/Game/{gameId}` | Game-specific top players report | ✅ |

---

## 📚 **API Usage Examples**

### **1. User Registration & Login**
```bash
# Create User
curl -X POST "https://localhost:7xxx/User/Create" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "player1",
    "email": "player1@example.com", 
    "password": "SecurePassword123"
  }'

# Login
curl -X POST "https://localhost:7xxx/Auth/Login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "player1",
    "password": "SecurePassword123"
  }'
```

### **2. Submit Scores**
```bash
curl -X PUT "https://localhost:7xxx/Score/Submit/Game/1/User/1?value=15000" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### **3. View Leaderboards**
```bash
# Global Leaderboard (Top 10)
curl -X GET "https://localhost:7xxx/Leaderboard/Global?pageIndex=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Game-Specific Leaderboard
curl -X GET "https://localhost:7xxx/Leaderboard/Game/1?pageIndex=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN"

# User's Ranking
curl -X GET "https://localhost:7xxx/Leaderboard/Rank/1?pageIndex=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### **4. Historical Reports**
```bash
# Top Players Report (Date Range)
curl -X GET "https://localhost:7xxx/Historical/TopPlayers/Global?fromDate=2024-01-01&toDate=2024-12-31&pageIndex=1&pageSize=10" \
  -H "Authorization: Bearer YOUR_TOKEN"

# Game-Specific Top Players
curl -X GET "https://localhost:7xxx/Historical/TopPlayers/Game/1?fromDate=2024-01-01&toDate=2024-12-31&limit=10" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 🏗️ **Architecture Overview**

### **Project Structure**
```
📁 Controllers/          # API Endpoints
📁 Services/             # Business Logic Layer  
📁 Interfaces/           # Service Contracts
📁 DataAccess/           # Database & Redis Integration
📁 Models/DTOs/          # Data Transfer Objects
📁 Helpers/              # Utility Classes
📁 Middleware/           # Cross-cutting Concerns
📁 Constants/            # Application Constants
📁 Migrations/           # EF Core Database Schema
```

### **Data Flow**
1. **Request** → Controller
2. **Authorization** → JWT Middleware  
3. **Business Logic** → Service Layer
4. **Authentication** → Token Service
5. **Caching** → Redis Cache Service
6. **Database** → Entity Framework Core
7. **Response** → DTO Mapping

### **Redis Integration**
- **Global Leaderboard**: `"global:leaderboard"`
- **Game Leaderboards**: `"game:{id}:leaderboard"`
- **User Rankings**: `"user:{id}:ranking"`
- **Real-time Updates**: Automatic via Redis Sorted Sets

---

## 🔧 **Advanced Configuration**

### **Environment-Specific Settings**

**Development (`appsettings.Development.json`)**
```json
{
  "ConnectionStrings": {
    "LeaderBoardConnection": "Server=localhost,1433;Database=LBDB_Dev;User=sa;Password=123;Trusted_Connection=true;TrustServerCertificate=true;",
    "RedisConnection": "localhost:6379,user=default,password=123"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Production Considerations**
- Use connection pooling for SQL Server
- Configure Redis clustering for high availability
- Implement proper JWT secret rotation
- Set up monitoring and logging

---

## 📊 **Performance Features**

### **Redis Optimizations**
- **Sorted Sets** for O(log N) ranking operations
- **Pagination** support for large leaderboards
- **Automatic Caching** of frequent queries
- **Background Sync** between Redis and SQL Server

### **Database Optimizations**
- **Temporal Tables** for historical data tracking
- **Indexed Queries** for fast score lookups
- **Optimized Joins** with Entity Framework
- **Connection Pooling** for scalability

---

## 🧪 **Testing the API**

### **Using Swagger UI**
1. Navigate to `/swagger` in development mode
2. Authorize with JWT token
3. Test endpoints interactively

### **Sample Test Data**
The application automatically seeds Redis with sample leaderboard data on startup (development mode).

---

## 🛡️ **Security Features**

- **JWT Authentication** with configurable expiration
- **Argon2 Password Hashing** - Industry-standard security
- **Input Validation** on all endpoints
- **SQL Injection Protection** via Entity Framework
- **CORS Configuration** for cross-origin requests
- **Global Exception Handling** for secure error responses

---

## 🚀 **Deployment Options**

### **Local Development**
```bash
dotnet run
```

### **Docker Deployment**
```dockerfile
# Create Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . /app
WORKDIR /app
RUN dotnet restore
RUN dotnet publish -c Release
ENTRYPOINT ["dotnet", "bin/Release/net8.0/2508-real-time-leaderboard-api.dll"]
```

### **Cloud Deployment**
- **Azure**: Container Apps, App Service
- **AWS**: ECS, Lambda
- **Google Cloud**: Cloud Run

---

## 📈 **Monitoring & Logging**

- **Structured Logging** with Serilog (configurable)
- **Health Checks** for Redis and SQL Server
- **Performance Counters** for key metrics
- **Error Tracking** with detailed exception handling

---

## 🤝 **Contributing**

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

---

## 📄 **License**

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🆘 **Support & Troubleshooting**

### **Common Issues**

**Redis Connection Failed**
- Ensure Redis server is running
- Check connection string configuration
- Verify firewall settings

**Database Migration Errors**
- Confirm SQL Server is accessible
- Check user permissions
- Validate connection string

**JWT Token Issues**
- Verify secret key configuration
- Check token expiration settings
- Ensure proper authorization header format

### **Getting Help**
- 📧 Email: [mescua@outlook.com]()
- 🐛 Issues: [GitHub Issues](https://github.com/mimescua/2508-real-time-leaderboard-api/issues)
- 📚 Documentation: [Wiki](https://github.com/mimescua/2508-real-time-leaderboard-api/wiki)

---

## 🎯 **Future Enhancements**

- [ ] **WebSocket Support** for real-time notifications
- [ ] **Caching Layers** for enhanced performance
- [ ] **Multi-Tenant Support** for different organizations
- [ ] **Advanced Analytics** with time-series data
- [ ] **Mobile SDK** for iOS/Android integration
- [ ] **Automated Testing** with comprehensive test suite

---

**Built with ❤️ using .NET 8 and Redis**