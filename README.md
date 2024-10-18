# Ticket Management System

This is a simple ticket management system built with Angular and .NET for managing tickets.

## Features

- List of tickets with basic information such as ticket ID, description, status, and creation date.
- Add new tickets with a description.
- Edit existing tickets with a description and status.
- Delete tickets from the list.
- Responsive pop-up form for adding and editing tickets.

## Technologies

- **Frontend**: Angular
- **Backend**: .NET (ASP.NET Core Web API)
- **Database**: SQL Server
- **HTTP Client**: Angular HttpClientModule for API communication
- **Form Handling**: Angular FormsModule

## Prerequisites

- Node.js (for Angular)
- .NET Core SDK
- SQL Server (or any database you're using)
- Angular CLI (for running Angular projects)

## Installation

1. **Backend**:
   - Navigate to the `backend` folder.
   - Install dependencies:
     ```bash
     dotnet restore
     ```
   - Update the `appsettings.json` with your SQL Server connection string.
   - Run the backend API:
     ```bash
     dotnet run
     ```

2. **Frontend**:
   - Navigate to the `frontend` folder.
   - Install Angular dependencies:
     ```bash
     npm install
     ```
   - Run the Angular frontend:
     ```bash
     ng serve
     ```

## Usage

- Go to `http://localhost:4200/` in your browser to interact with the app.
- The main page allows you to view, add, edit, and delete tickets.
- Use the form to add new tickets or edit existing ones.

## API Endpoints (Backend)

- **GET** `/api/tickets`: Fetch all tickets.
- **POST** `/api/tickets`: Add a new ticket.
- **PUT** `/api/tickets/{id}`: Update a ticket by its ID.
- **DELETE** `/api/tickets/{id}`: Delete a ticket by its ID.

## Angular Components

- **TicketListComponent**: Displays the list of tickets and handles actions like add, edit, and delete.

## Contributing

Feel free to submit pull requests for improvements and bug fixes.
