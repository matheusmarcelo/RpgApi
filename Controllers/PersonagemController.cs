using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RpgApi.Models;
using RpgApi.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RpgApi.Controllers
{
    [Authorize(Roles="Jogador, Admin")]
    [ApiController]   
    [Route("[controller]")] 
    public class PersonagemController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PersonagemController(DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int ObterUsuarioId()
        {            
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private string ObterPerfilUsuario()
        {
            return _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);            
        }


        [HttpPost]
        public async Task<IActionResult> AddPersonagemAsync(Personagem novoPersonagem)
        {
            novoPersonagem.Usuario = await _context.Usuarios.FirstOrDefaultAsync(uBusca => uBusca.Id == ObterUsuarioId());            

            //Salvamento dos dados
            await _context.Personagens.AddAsync(novoPersonagem);
            await _context.SaveChangesAsync();

            List<Personagem> listaPersonagens = await _context.Personagens.ToListAsync();
            
            return Ok(listaPersonagens);
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePersonagemAsync(Personagem p)
        {
            p.Usuario = await _context.Usuarios.FirstOrDefaultAsync(uBusca => uBusca.Id  == ObterUsuarioId());           
            
            _context.Personagens.Update(p);
            await _context.SaveChangesAsync();

            return Ok(p);            
        } 

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingleAsync(int id)
        {
            Personagem p =  await _context.Personagens 
            .Include(u => u.Usuario)           
            .Include(ar => ar.Arma)
            .Include(ph => ph.PersonagemHabilidades).ThenInclude(h => h.Habilidade)            
            .FirstOrDefaultAsync(b => b.Id == id);
            return Ok(p);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAsync()
        {
            List<Personagem> listaPersonagens = new List<Personagem>();

            if(ObterPerfilUsuario() == "Admin")
            {
                listaPersonagens = await _context.Personagens
                .Include(u => u.Usuario)
                .ToListAsync();
            }
            else
                listaPersonagens = await _context.Personagens
                    .Where(p => p.Usuario.Id == ObterUsuarioId()).ToListAsync();

            return Ok(listaPersonagens);
        }

        [HttpGet("GetByUser")]
        public async Task<IActionResult> GetByUserAsync()
        {
            int id = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            List<Personagem> listaPersonagens = await _context.Personagens.ToListAsync();
            return Ok(listaPersonagens);
        }
             
        

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(Personagem p)
        {
            Personagem pRemover = 
            await _context.Personagens.FirstOrDefaultAsync(pBusca => pBusca.Id == p.Id && pBusca.Forca ==50);

            _context.Personagens.Remove(pRemover);
            await _context.SaveChangesAsync();

            List<Personagem> listPersonagens = await _context.Personagens.ToListAsync();

            return Ok(listPersonagens);
        }

        [HttpPut("RestaurarPontosVida")]
        public async Task<IActionResult> RestaurarPontosVidaAsync(Personagem p)
        {
            Personagem pEncontrado = 
                await _context.Personagens.FirstOrDefaultAsync(pBusca => pBusca.Id == p.Id);           

            pEncontrado.PontosVida = 100;
            
            bool atualizou = await TryUpdateModelAsync<Personagem>(pEncontrado, "p",
                pAtualizar => pAtualizar.PontosVida);
            
            // EF vai detectar e atualizar apenas as colunas que foram alteradas.
            if (atualizou)                            
                await _context.SaveChangesAsync();
            
            return Ok(pEncontrado);            
        } 

        

        [HttpPut("ZerarRanking")]
        public async Task<IActionResult> ZerarRankingAsync(Personagem p)
        {
            Personagem pEncontrado = 
                await _context.Personagens.FirstOrDefaultAsync(pBusca => pBusca.Id == p.Id);           

            pEncontrado.Disputas = 0;
            pEncontrado.Vitorias = 0;
            pEncontrado.Derrotas = 0;

            
            bool atualizou = await TryUpdateModelAsync<Personagem>(pEncontrado, "p",
                pAtualizar => pAtualizar.Disputas,
                pAtualizar => pAtualizar.Vitorias,
                pAtualizar => pAtualizar.Derrotas);
            
            // EF vai detectar e atualizar apenas as colunas que foram alteradas.
            if (atualizou)                            
                await _context.SaveChangesAsync();
            
            return Ok(pEncontrado);            
        } 

        [HttpPut("ZerarRankingGeral")]
        public async Task<IActionResult> ZerarRankingGeralAsync()
        {
            List<Personagem> lista = 
                await _context.Personagens.ToListAsync();           

            foreach(Personagem p in lista)
            {
                await ZerarRankingAsync(p);
            }                     
            return Ok();            
        } 

        

        









        



        
                       
    }
}

