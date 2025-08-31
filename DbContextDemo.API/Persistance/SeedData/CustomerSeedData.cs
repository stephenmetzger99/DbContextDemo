namespace DbContextDemo.API.Persistance.SeedData;

using DbContextDemo.API.Domain;
using System.Collections.Generic;

public static class CustomerSeedData
{
    public static List<Customer> GetSeedCustomers() => new()
    {
        new() {  Name = "Alice Smith", Email = "alice1@example.com" },
        new() {  Name = "Bob Johnson", Email = "bob2@example.com" },
        new() {  Name = "Carol Williams", Email = "carol3@example.com" },
        new() {  Name = "David Brown", Email = "david4@example.com" },
        new() {  Name = "Eve Jones", Email = "eve5@example.com" },
        new() {  Name = "Frank Miller", Email = "frank6@example.com" },
        new() {  Name = "Grace Wilson", Email = "grace7@example.com" },
        new() {  Name = "Hank Moore", Email = "hank8@example.com" },
        new() {  Name = "Ivy Taylor", Email = "ivy9@example.com" },
        new() { Name = "Jack Anderson", Email = "jack10@example.com" }
    };
}
