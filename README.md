 # Helper that gives Azure Cognitive search a LINQ like API
 
## Core

### Abstractions

Abstraction project is where all global interfaces are

### Application

Application project is where the application interfaces and some concrete abstraction
implementations are

### Extensions

Extension project is where the base linq like extension methods are

## Query Runners

### V1

First implementation of a query runner, it has a lot of things that can be improved its mostly in POC state

## Helpers

### OData

Basic implementation of Odata filters

## Dependency Injection

### Standard DI

project that use default dotnet DI to inject a context
