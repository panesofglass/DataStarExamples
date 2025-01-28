# DataStar Examples
### built using [DataStar](https://data-star.dev/) and [ASP.NET Core Web Apps](https://dotnet.microsoft.com/en-us/apps/aspnet)

This repository contains three implementations of active search functionality using DataStar:

1. *working* > An [ASP.NET Core Web App](https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-9.0) (Model-View-Controller)
2. ****not*** working* > An [ASP.NET Core Web App](https://learn.microsoft.com/en-us/aspnet/core/data/ef-rp/intro?view=aspnetcore-9.0&tabs=visual-studio) (Razor Pages)
  *for Razor Pages > getting a 400 Bad Request error when passing in http://localhost:5019/Index?handler=Search*
3. *working* > An [ASP.NET Core Web App](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-9.0) (Minimal APIs)
4. ****not*** setup yet* > A [FastEndpoints](https://fast-endpoints.com/docs/server-sent-events) project

## Demo > Active Search
![Active Search Demo](mvc-datastar-examples.png)  

## What is DataStar?

DataStar is a lightweight hypermedia framework that brings reactive functionality to server-rendered applications. It combines the best of both worlds:

- The simplicity and reliability of server-side rendering
- The dynamic, reactive user experience in the style of a Single Page Application (SPA) but without the need for JavaScript

## Benefits

- **Build Like a Pro, Code Like a Beginner**: Create sophisticated, reactive web applications without wrestling with complex state management or JavaScript frameworks
- **Real-Time Magic**: Update multiple parts of your page instantly - imagine a chat application where messages, user lists, and notifications all update in real-time without writing a single line of JavaScript
- **Your Server, Your Rules**: Keep all your business logic where it belongs - on the server. No more duplicating validation rules or business logic in JavaScript
- **Learn Once, Build Anything**: If you can build a traditional web page, you can build reactive applications. No need to learn Redux, React, or complex state management patterns
- **Instant Feedback**: Create responsive interfaces that feel like native apps - type in a search box and watch results filter instantly, all without complex client-side code
- **Focus on Features, Not Plumbing**: Spend your time building features users love instead of managing state, writing JavaScript, or debugging client-server communication
- **Say Goodbye to Full Page Refreshes**: The dreaded full page refresh is a thing of the past with DataStar - enjoy smooth, partial updates that keep your users in flow

## Why Use DataStar?

- **Minimal JavaScript**: Just include a single 12.9 KiB file - smaller than Alpine.js and Htmx combined
- **HATEOAS Compliant**: The only JavaScript you need is the library itself - no additional client-side code required
- **Server Technology Agnostic**: Write your backend in any language
- **Real-time Updates**: Utilizes server-sent events for fast, responsive experiences

## About This Example

This repository demonstrates DataStar's capabilities through an active search implementation. Users can type search queries and see results update in real-time, with all the heavy lifting handled server-side. Both the MVC and Razor Pages implementations achieve the same functionality using their respective architectural patterns.

## Getting Started

Both implementations demonstrate how to:
- Set up DataStar in an [ASP.NET Core Web App](https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-9.0)
- Implement real-time search functionality
- Handle server-sent events
- Structure your application for reactive behavior

For more details and documentation, visit the [DataStar Active Search Example](https://data-star.dev/examples/active_search)

## Community

Join our growing community of developers:

- **Discord**: Join our [Discord server](https://discord.gg/bnRNgZjgPh) to:
  - Get help with your DataStar projects
  - Share your experiences and learn from others
  - Stay updated on the latest features and best practices
  - Connect with fellow developers

- **YouTube**: Subscribe to our [YouTube channel](https://www.youtube.com/@data-star) for:
  - Tutorial videos
  - Feature demonstrations
  - Best practices
  - Implementation examples

- **GitHub**: Check out our [library source code](https://github.com/starfederation/datastar/tree/main/library) to:
  - Explore the implementation
  - Contribute to the project
  - Report issues
  - Star the repository