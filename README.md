Phone number +14848239774 is successfully deleted.

# PhloSystemsAssessment

## Overview

This project provides an API for filtering products based on various parameters such as price, size, and more. The API allows clients to easily retrieve and manipulate product data in a structured format.

## API Documentation

The API is designed to be intuitive and easy to use. Below is a summary of the available endpoints.

### Base URL

### Endpoints

#### 1. Filter Products

- **Endpoint:** `/filter`
- **Method:** `GET`
- **Description:** Retrieve filtered products based on specified criteria.

**Query Parameters:**

| Parameter   | Type       | Description                                                                        |
| ----------- | ---------- | ---------------------------------------------------------------------------------- |
| `id`        | `int?`     | (Optional) Filter products by a specific ID.                                       |
| `minPrice`  | `decimal?` | (Optional) Minimum price to filter products.                                       |
| `maxPrice`  | `decimal?` | (Optional) Maximum price to filter products.                                       |
| `size`      | `string`   | (Optional) Filter products by size (e.g., "small").                                |
| `highlight` | `string`   | (Optional) Comma-separated list of words to highlight in the product descriptions. |

**Response:**

- **Success Response:** HTTP Status Code `200 OK`

```json
{
  "Products": [
    {
      "Id": 1,
      "Name": "Green T-shirt",
      "Price": 15,
      "Sizes": ["medium"],
      "Description": "A nice green t-shirt"
    },
    ...
  ],
  "Filter": {
    "MinPrice": 10,
    "MaxPrice": 25,
    "Sizes": ["small", "medium", "large"],
    "CommonWords": ["nice", "green", "t-shirt"]
  }
}
Error Responses:

Invalid Query Parameters: HTTP Status Code 400 Bad Request

{
  "error": "Invalid query parameters."
}
Example Requests

1. Get all products:
GET /filter

2. Filter products by price range:
GET /filter?minPrice=10&maxPrice=20

3. Filter products by size and highlight words:
GET /filter?size=medium&highlight=green,shirt

Testing the API
You can test the API endpoints using tools like Postman or cURL.

Contributing
If you would like to contribute to this project, please follow these steps:

Fork the repository.
Create a new branch (git checkout -b feature/YourFeature).
Make your changes.
Commit your changes (git commit -m 'Add some feature').
Push to the branch (git push origin feature/YourFeature).
Open a Pull Request.

Contact
For any questions or inquiries, please reach out to me.


### Summary

This template provides a comprehensive overview of your API, including details about endpoints, parameters, example requests, and more. You can customize it further based on the specific features and functionality of your API.
```
