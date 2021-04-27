using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;
using Microsoft.AspNetCore.Authorization;



namespace Shop.Controlers
{
    [Route("users")]
    public class UserController : Controller
    {


            [HttpGet]
            [Route("")]
            [Authorize(Roles = "manager")] //somente o gerente pode acessar os dados de usuários
            public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
            {
                var users = await context
                    .Users
                    .AsNoTracking()
                    .ToListAsync();

                return users;
            }



            [HttpPost]
            [Route("")]
            [AllowAnonymous]
            public async Task<ActionResult<User>> Post(
                [FromServices] DataContext context,
                [FromBody]User model)
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);


                try
                {
                    //força o usuário a ser sempre "funcionário"
                    model.Role = "employee";

                    context.Users.Add(model);
                    await context.SaveChangesAsync();

                    //Esconde a senha
                    model.Password = "";

                    return model;       
                }
                catch (Exception)
                {
                    return BadRequest(new {message = "Não foi possível criar o usuário"});
                }
            }   


            [HttpPost]
            [Route("login")]
            // usando dynamic porque as vezes ele retornará um usuário e as vezes não retornará nada
            public async Task<ActionResult<dynamic>> Authenticate(
                [FromServices] DataContext context,
                [FromBody]User model)
            {
                var user = await context.Users
                    .AsNoTracking()
                    .Where(x => x.Username == model.Username && x.Password == model.Password)
                    .FirstOrDefaultAsync();

                //Vai no banco e se não encontrar o usuário ele avisa
                if(user == null)
                    return NotFound(new { message = "Usuário ou senha inválidos"});

                //Validação simples
                var token = TokenService.GenerateToken(user);

                //esconde a senha
                user.Password = "";
                return new
                {
                    user = user,
                    token = token
                };
            }

            [HttpPut]
            [Route("{id:int")]
            [Authorize(Roles = "manager")]
            public async Task<ActionResult<User>> Put(
                [FromServices] DataContext context,
                int id,
                [FromBody]User model)
            {
                //verifica se os dados são válidos
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //verifica se o ID informado é o mesmo do modelo
                if (id != model.Id)
                    return NotFound( new{ message = "Usuário não encontrado"});

                try
                {
                    context.Entry(model).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return model;
                }
                catch
                {
                     return BadRequest(new {message = "Não foi possível criar o usuário"});
                }
            }


    }
}