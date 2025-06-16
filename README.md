Cartify- Shopping Cart API
This is a simple ASP.NET Core Web API project that implements a Shopping Cart system with Authentication and Authorization. It allows users to register/login, browse products, and manage their shopping cart.

📦 Features
✅ User Authentication (via ASP.NET Identity)

✅ Products and Categories management

✅ Shopping Cart linked to each authenticated user

✅ Secure API endpoints with [Authorize]

✅ Add, remove, and view items in your cart

✅ Filter products by category

📁 Project Structure
markdown
Copy
Edit
/Models
  - Product.cs
  - Category.cs
  - ShoppingCart.cs

/Controllers
  - ProductController.cs
  - ShoppingCartController.cs

/Data
  - ApplicationDbContext.cs
  - SecurityDbContext.cs
🚀 Endpoints
🔐 Authorization
All endpoints require the user to be authenticated.

Product EndPoints:
| Method | Endpoint                     | Description                 |
| ------ | ---------------------------- | --------------------------- |
| GET    | `/api/product`               | Get all products            |
| GET    | `/api/product/category/{id}` | Get products by category ID |
| POST   | `/api/product`               | Add a new product           |

Shopping cart Endpoints
| Method | Endpoint                 | Description                            |
| ------ | ------------------------ | -------------------------------------- |
| GET    | `/api/shoppingcart`      | Get current user's cart items          |
| POST   | `/api/shoppingcart/{id}` | Add product to cart by product ID      |
| DELETE | `/api/shoppingcart/{id}` | Remove product from cart by product ID |

🧰 Technologies Used
C#

ASP.NET Core Web API

Entity Framework Core

Microsoft Identity (for authentication)

JWT (optional for token-based auth)


