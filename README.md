
## Prerequisites

- **Backend:**
  - [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or later.
  - Visual Studio Code with the C# extension.
- **Frontend:**
  - [Node.js](https://nodejs.org/) (preferably v16+).
  - npm (or yarn) installed.
- **Testing:**
  - xUnit (configured via the .NET test project).

## Setup and Running the Application

### Backend (API)

1. **Navigate to the Backend Directory:**

   ```bash
   cd Backend/UserProximity.API

2. **Restore Dependencies and Run the API:**
   ```bash
   dotnet restore
   dotnet run
4. **Running unit tests:**
   ```bash
   cd Backend/NearestUserServiceTests
   dotnet test
5. **Running Frontend:**
    ```bash
    cd Frontend
    npm install
    npm run dev
