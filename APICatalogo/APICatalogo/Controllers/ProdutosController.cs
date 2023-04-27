using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;  //gerando uma instancia de cotnext

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    //api/produtos/primeiro
    //primeiro
    [HttpGet("primeiro")]
    [HttpGet("/primeiro")]
    public IActionResult GetPrimeiro() { 
        var produto = _context.Produtos.FirstOrDefault();
        if (produto == null) {
            return NotFound();
        }
        return Ok(produto);
    }
    //api/produtos
    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    public async Task<ActionResult<IEnumerable<Produto>>> Get() { 
        var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
        if(produtos is null)
        {
            return NotFound("Produto nao encontrados");
           
        }
        return produtos;
    }
    
    //api/produtos/1
    [HttpGet("{id}", Name="ObterProduto")]
    public async Task<ActionResult<Produto>> Get([FromQuery]int id)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);
        if(produto == null)
        {
            return NotFound("Produto com id nao encontrados");
        }
        return produto;
    }

    //api/produtos
    [HttpPost]
    public ActionResult Post(Produto produto){
        if(produto is null){
            return BadRequest();
        }
        _context.Produtos.Add(produto); //trabalhando na memoria
        _context.SaveChanges(); //realiznado de fato a persistencia dos dados

        return new CreatedAtRouteResult("ObterProduto",  
            new { id = produto.ProdutoId }, produto);
    }

    //api/produtos/1
    [HttpPut("{id:int}")]
    public ActionResult Put(int id, Produto produto)
    {
        if(id != produto.ProdutoId)
        {
            return BadRequest();
        }

        _context.Entry(produto).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(produto);

    }

    //api/produtos/1
    [HttpDelete ("{id:int}")]
    public ActionResult Delete(int id) {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
        if(produto is null)
        {
            return NotFound("Produto não localizado...");
        }
        _context.Produtos.Remove(produto);
        _context.SaveChanges();
        return Ok(produto);
    }

}
