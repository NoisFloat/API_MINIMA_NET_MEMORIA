using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PizzaStore.Modelos;
var builder = WebApplication.CreateBuilder(args);


//
builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));
//
builder.Services.AddEndpointsApiExplorer();
/**********************************************/
//Le das configuracion base a Swagger(haces que exista)
builder.Services.AddSwaggerGen(c =>
{
    //Este metodo, no es necesario como tal, pero siempre es bueno tener un versionamiento de nuestro Software

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pizza Store Api", Description = "Making the Pizzas you love", Version = "v1" });
});

var app = builder.Build();
/*Este if, REUNE los datos que toma para hacer la pagina, el json en base a los Endpoint*/
if (app.Environment.IsDevelopment())
{
    //Activa la pagina de Swagger
    app.UseSwagger();
    //En base a toda la informacion obtenido de los Endpoint y de Configuracion, ahora creara un Json con toda la data, que luego sera usada para la interfaz grafica de 
    //Swagger
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1"); });
}

app.MapGet("/", () => "Hello World!");
app.MapGet("/fecha", () => DateTime.Now.ToString("o"));
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});
/*Insersion Asincronica*/
app.MapPost("/pizzaAdd", async (PizzaDb db, Pizza pizza) =>
{
    Pizza simuladaRespuesta = new Pizza
    {
        Id = 10,
        Name = "HawaiCampero",
        Description = "Si esto funciona significa que RESULTS, metodo del return," +
                                                                                        " realmente lo unico que hace es retornarte la misma informacion si se creo el recurso"
    };
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", simuladaRespuesta);
});
//Obtencion Asincronica
app.MapGet("pizza/{id}", async (PizzaDb db, int id) =>
{
    //Este automaticamente genera un codigo de respuesta si encuentra la pizza

    return await db.Pizzas.FindAsync(id);

});
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatePizza, int id) => { 
    
    var pizza = db.Pizzas.Find(id);
    if (pizza == null) return Results.NotFound();
    pizza.Name = updatePizza.Name;
    pizza.Description = updatePizza.Description;
    await db.SaveChangesAsync(); //Directamente hace un UPDATE
    return Results.NoContent();
    

});


app.MapDelete("/pizza/{id}", async (PizzaDb db, int? id) => { 
var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) { 
    return Results.NotFound();
    }
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.Run();
