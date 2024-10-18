<img width="311" alt="Screenshot 2022-07-30 at 22 51 12" src="https://user-images.githubusercontent.com/7254177/182964758-f50c59fb-a635-43e5-b520-0319e7e15025.png">

**Clouded: Modular Cloud Platform**

**Overview:**  
Clouded is a cloud platform I developed to streamline the management of cloud-based applications, focusing on flexibility and scalability. Built in C# with Docker and Kubernetes, Clouded integrates several key services, called Providers, to handle essential functions like authentication, admin management, and cloud functions.

**Key Features:**

- **Auth Provider:** A customizable authentication system, comparable to Auth0 and Keycloak. It manages users, roles, organizations, and permissions, with support for machine-to-machine tokens (M2M) and SSO login (Google, Facebook, Apple). The provider offers JavaScript and C# libraries for easy integration and includes an admin UI with CRUD operations.
  
- **Admin Provider:** A flexible admin tool that connects to existing databases, allowing you to configure and manage database tables and relationships quickly. The admin interface is fully customizable and supports multiple databases at once, making data management simple and efficient.

- **Cloud Functions (In Development):** Language-agnostic functions that can extend providers with custom actions (e.g., sending emails when a user is created). Users can define these functions, integrate them via Git, and assign them to specific events.

- **API Provider (In Development):** An upcoming feature that will automatically generate CRUD APIs from database configurations. It will also allow custom endpoints and extend API functionality with cloud functions, offering flexibility similar to AWS Lambda.

**Why Clouded?**  
Clouded integrates essential cloud services into a unified, modular system, removing the complexity of managing separate tools. It enables rapid deployment and easy management, allowing businesses to focus on building innovative features without worrying about infrastructure.