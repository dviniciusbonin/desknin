## Architecture

DeskNin starts as a **monolithic application built with ASP.NET Core MVC**, prioritizing simplicity and fast development. The architecture is designed to allow a gradual evolution toward an **API + SPA** model in the future.

### Architecture (V1)

The first version uses **server-side rendering** with Razor Views.

```
Browser
   ↓
ASP.NET Core MVC Controllers
   ↓
Application Services
   ↓
Entity Framework Core
   ↓
Database
```

**Main Components**

* **Controllers**
  Responsible for handling HTTP requests and returning Views.

* **Services**
  Contain the application's business logic.

* **Repositories / Data Layer**
  Responsible for data access using Entity Framework Core.

* **Razor Views**
  Server-side rendering of application pages.

* **Authentication**
  Implemented using ASP.NET Identity.

### Project Structure

```
DeskNin
│
├ Controllers
├ Services
├ Models
├ ViewModels
├ Data
├ Views
└ wwwroot
```

### Authentication & Authorization

Authentication is implemented using **ASP.NET Identity** with a global login based on email and password.

Role-based access control:

* **Admin** – user and system configuration management
* **Agent** – responsible for resolving tickets
* **User** – creates and tracks tickets

### Multi-Tenant Design (V2)

In the second phase, the system evolves into a **multi-tenant architecture**, allowing multiple companies to use the same platform while keeping their data isolated.

Each user belongs to a company (workspace):

```
Company
   └ Users
   └ Tickets
   └ Comments
```

All operations are filtered by `CompanyId`.

### Possible Future Architecture (V3+)

The frontend may be migrated to a SPA using:

* Next.js
* Angular

In this scenario, the architecture evolves to:

```
Frontend (SPA)
   ↓
ASP.NET Core REST API
   ↓
Application Services
   ↓
Database
```

This approach enables:

* greater frontend flexibility
* improved user experience
* clear separation between frontend and backend
* better application scalability

### Infrastructure (Lab Environment)

This project also serves as a laboratory for modern infrastructure practices, with future support for:

* Docker
* Kubernetes
* CI/CD
* Cloud deployment