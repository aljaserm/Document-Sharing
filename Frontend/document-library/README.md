# Document Library

The Document Library is a web-based solution for storing and sharing documents. Users can upload various types of documents, view a list of available documents, download individual or multiple documents, and share documents via generated links.

## Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Running the Application](#running-the-application)
- [Architecture and Design](#architecture-and-design)
- [Future Improvements](#future-improvements)

## Features

- Upload documents (PDF, Excel, Word, txt, images)
- Display a list of available documents
- Show document details (name, icon, preview image, upload date, download count)
- Download individual documents
- Download multiple documents
- Generate and share document links with a specified expiration time
- Containerized deployment using Docker

## Technologies Used

- Frontend: React
- Backend: ASP.NET Core
- Database: SQL Server
- Containerization: Docker

## Project Structure

```
DocumentSharing/
|-- Backend
|   |-- DocumentLibrary
|       |-- Application
|       |-- Infrastructure
|       |-- Test
|       |-- Web
|-- Frontend
|   |-- document-library
|-- deploy
|-- .env
|-- Dockerfile.backend
|-- Dockerfile.database
|-- Dockerfile.frontend
|-- docker-compose.yml
```

## Getting Started

### Prerequisites

- [Docker](https://www.docker.com/get-started) installed on your machine
- [Node.js](https://nodejs.org/) and npm installed
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed

### Running the Application

#### Using Docker

1. **Clone the repository**

   ```sh
   git clone https://github.com/your-repository/DocumentSharing.git
   cd DocumentSharing
   ```

2. **Set up environment variables**

   Create a `.env` file in the `deploy` directory with the following content:

   ```env
   SA_PASSWORD=YourPassword123
   ACCEPT_EULA=Y
   ASPNETCORE_ENVIRONMENT=Development
   ```

3. **Build and run the application using Docker Compose**

   ```sh
   cd deploy
   docker-compose up --build
   ```

   This command will:
    - Build and run the SQL Server database container
    - Build and run the backend ASP.NET Core application container
    - Build and run the frontend React application container

4. **Access the application**

    - Frontend: [http://localhost:3000](http://localhost:3000)
    - Backend API: [http://localhost:5000](http://localhost:5000)

#### Using .NET and npm Directly

1. **Clone the repository**

   ```sh
   git clone https://github.com/your-repository/DocumentSharing.git
   cd DocumentSharing
   ```

2. **Set up the database**

   Ensure SQL Server is running locally or update the connection string in `appsettings.Development.json` with your database configuration. Run the migrations to set up the database schema:

   ```sh
   cd Backend/DocumentLibrary
   dotnet ef database update
   ```

3. **Run the backend**

   Navigate to the backend project directory and start the ASP.NET Core application:

   ```sh
   cd Backend/DocumentLibrary/Web
   dotnet run
   ```

   The backend API will be accessible at [http://localhost:5000](http://localhost:5000).

4. **Run the frontend**

   Navigate to the frontend project directory, install the dependencies, and start the React application:

   ```sh
   cd Frontend/document-library
   npm install
   npm start
   ```

   The frontend will be accessible at [http://localhost:3000](http://localhost:3000).

## Architecture and Design

The application is divided into two main parts: the frontend and the backend.

### Frontend

- The frontend is built with React and handles the user interface and interactions.
- The main components include:
    - DocumentPage: Displays the list of documents and handles upload/download actions.
    - UploadPage: Provides a form for uploading new documents.
    - HomePage: The landing page of the application.
    - ShareModal: A modal for generating and copying share links.

### Backend

- The backend is built with ASP.NET Core and provides a RESTful API for managing documents.
- The main components include:
    - Commands and Queries for handling different operations such as uploading, downloading, and generating share links.
    - Entity Framework Core is used for database operations.

### Database

- SQL Server is used as the database to store document information and share links.

### Containerization

- Docker is used to containerize the frontend, backend, and database.
- Docker Compose is used to orchestrate the multi-container setup.

## Future Improvements

- **Preview Image**: Implement preview image generation for the first page of documents.
- **Multi-file Upload**: Add functionality to upload multiple documents at once.
- **Unit Tests**: Implement unit tests for both the frontend and backend.
- **Shared Link View**: Create a dedicated view for accessing documents via shared links.

## Running Tests

To run tests for the backend:

```sh
cd Backend/DocumentLibrary
dotnet test
```

To run tests for the frontend:

```sh
cd Frontend/document-library
npm install
npm test
```

## Conclusion

This project meets most of the requirements specified, with a few functionalities left as future improvements. The additional containerization showcases the ability to manage projects from development to deployment efficiently.