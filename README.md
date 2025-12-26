# DbContextDemo

This project's goal is to illustrate how the dbcontext acts as a unit of work. 

This project does not aim to be a perfect reference architecture. 

Certain tradeoffs were made for purposes of simplifying the demo. 

For one, the whole backend is one project. 
Layers were simplified and broken up into folders.

It is broken into
* Api Layer
* Service layer
  There is only one service, which plays a central role to this demo. 
  Most of the endpoints bypass the service layer, except the POST /orders endpoint
* Peristance
  Most of the work was done here
  There are multiple Repos that use dbcontext differently.
   

A standard generic repository is used. Generic repos arent ideal but they suffice for a quick demo. 