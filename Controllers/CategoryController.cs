using Microsoft.AspNetCore.Mvc;


//EndPoint => URL
//https://localhost:5001/categories
//http://localhost:5000

[Route("categories")]
public class CategoryController : ControllerBase 
{

    //https://localhost:5001/categories
    [Route("")]
    public string MeuMetodo()
    {
        return "Olá mundo";
    }
}