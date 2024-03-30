# Cooking Recipes API

## Project Overview

The Cooking Recipes API is a culinary platform allowing users to create, manage, and explore cooking recipes and their corresponding ingredients. It incorporates user authentication, ensuring that read operations are publicly accessible while write operations require user authentication. This security measure mandates that users must be registered within the system to perform any modifications.

### Database Configuration

This API uses Microsoft SQL Server, leveraging the Docker image `mcr.microsoft.com/mssql/server:2022-latest` for database management. To access the database, use the following command:

```sh
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin1234" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

- **Username:** sa
- **Password:** Admin1234

**Important:** For Docker deployments, replace the `Server` address in the connection string with the IP address of the machine hosting the Docker container. You can find the IP address of your machine by running `ifconfig` on Linux and Mac or `ipconfig` on Windows in the terminal or command prompt.

#### Setting Up the Database

Execute the following SQL script using your preferred database management tool (e.g., DBeaver, DataGrip) to set up the schema.

Before proceeding, make sure you are targeting the `model` database in your SQL Server instance.

**Warning:** Ensure you are operating on the correct database to avoid unintended alterations.

```sql
CREATE TABLE Users (
    UserID INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(255) NOT NULL,
    PasswordHash VARBINARY(256) NOT NULL,
    PasswordSalt VARBINARY(256) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
);

CREATE TABLE Recipes (
    RecipeID INT IDENTITY PRIMARY KEY,
    UserID INT NOT NULL,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Instructions NVARCHAR(MAX) NOT NULL,
    PrepTime INT NOT NULL,
    CookTime INT NOT NULL,
    Servings INT NOT NULL,
    CONSTRAINT FK_Recipes_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE Ingredients (
    IngredientID INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL
);

CREATE TABLE RecipeIngredients (
    RecipeID INT NOT NULL,
    IngredientID INT NOT NULL,
    Quantity DECIMAL(5,2) NOT NULL,
    MeasureUnit NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_RecipeIngredients PRIMARY KEY (RecipeID, IngredientID),
    CONSTRAINT FK_RecipeIngredients_Recipes FOREIGN KEY (RecipeID) REFERENCES Recipes(RecipeID),
    CONSTRAINT FK_RecipeIngredients_Ingredients FOREIGN KEY (IngredientID) REFERENCES Ingredients(IngredientID)
);
```

### Project Configuration
To get started with the Cooking Recipes API, follow these steps:

#### Clone the repository:
```bash
git clone https://github.com/juanmafernandezc/cooking-recipes-api.git
```

#### Navigate to the project's root directory:
```bash
cd cooking-recipes-api
```

#### You'll need to set up the following environment variables within the project:
```json
"environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development",
    "DatabaseConnectionString": "Server=localhost;Database=model;User Id=sa;Password=Admin1234;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;",
    "JwtKey": "",
    "JwtIssuer": "",
    "JwtAudience": ""
}
```

**Note**: The JwtKey, JwtIssuer, and JwtAudience values are placeholders. You will need to generate and provide your own values for these variables, as they are not included in the repository for security reasons.

#### Running the Project
To run the project locally, simply deploy it using the provided profile within your development environment. For Docker deployment, execute the following commands from the project root:

```sh
docker build -t cookingrecipes.api .
docker run -d -p 8080:8080 --name mycookingapi -e "DatabaseConnectionString=Server=your_ip;Database=model;User Id=sa;Password=Admin1234;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;" -e "JwtKey=jwt_key" -e "JwtIssuer=jwt_issuer" -e "JwtAudience=jwt_audience" cookingrecipes.api
```

### API Documentation and Collection

For ease of testing and exploring the Cooking Recipes API, a Postman collection is provided with pre-configured requests for each available endpoint [here](./postman/collection.json).