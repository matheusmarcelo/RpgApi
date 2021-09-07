using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RpgApi.Models;
using RpgApi.Models.Enuns;
using System.Linq;

namespace RpgApi.Controllers
{
    [ApiController]   
    [Route("[controller]")]
    public class TesteController : ControllerBase
    {
         private static List<Personagem> personagens = new List<Personagem>() {            
            new Personagem() { Id = 1, }, //Frodo Cavaleiro             
            new Personagem() { Id = 2, Nome = "Sam", PontosVida=100, Forca=15, Defesa=25, Inteligencia=30, Classe=ClasseEnum.Cavaleiro},     
            new Personagem() { Id = 3, Nome = "Galadriel", PontosVida=100, Forca=18, Defesa=21, Inteligencia=35, Classe=ClasseEnum.Clerigo },
            new Personagem() { Id = 4, Nome = "Gandalf", PontosVida=100, Forca=18, Defesa=18, Inteligencia=37, Classe=ClasseEnum.Mago },
            new Personagem() { Id = 5, Nome = "Hobbit", PontosVida=100, Forca=20, Defesa=17, Inteligencia=31, Classe=ClasseEnum.Cavaleiro },
            new Personagem() { Id = 6, Nome = "Celeborn", PontosVida=100, Forca=21, Defesa=13, Inteligencia=34, Classe=ClasseEnum.Clerigo },
            new Personagem() { Id = 7, Nome = "Radagast", PontosVida=100, Forca=25, Defesa=11, Inteligencia=35, Classe=ClasseEnum.Mago }       
        }; 

        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            return Ok(personagens);
        }

        public IActionResult GetSingle()
        {
            return Ok(personagens[0]);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            Personagem pEncontrado = personagens.FirstOrDefault(p => p.Id == id);
            return Ok(pEncontrado);            
            //return Ok(personagens.FirstOrDefault(p => p.Id == id));
        }

        [HttpGet("GetCombinacao")] //Força menor que 20 e Inteligência maior que 30
        public IActionResult GetCombinacao(int id)
        {
            List<Personagem> listaEncontrada = 
                personagens.FindAll(p => p.Forca < 20 && p.Inteligencia > 30 );

            return Ok(listaEncontrada);              
        }


        [HttpGet("GetOrdenado")]
        public IActionResult GetOrdem()
        {
            List<Personagem> listaOrdenada = personagens.OrderBy(p => p.Forca).ToList();
            return Ok(listaOrdenada);
        }

        [HttpGet("GetContagem")]
        public IActionResult GetQuantidade()
        {
            return Ok("Quantidade de personagens: " + personagens.Count);
        }

        [HttpGet("GetSomaForca")]
        public IActionResult GetSomaForca()
        {
            return Ok(personagens.Sum(p => p.Forca));
        }

        [HttpGet("GetSemCavaleiro")]
        public IActionResult GetSemCavaleiro()
        {
            List<Personagem> listaBusca = personagens.FindAll(p => p.Classe != ClasseEnum.Cavaleiro);
            return Ok(listaBusca);
        }

        [HttpGet("GetByNomeAproximado/{nome}")]
        public IActionResult GetByNomeAproximado(string nome)
        {
            //List<Personagem> listaBusca = personagens.FindAll(p => p.Nome.Contains(nome));
            List<Personagem> listaBusca = personagens.FindAll(p => p.Nome == nome);

            return Ok(listaBusca);
        }

        [HttpGet("GetRemovendoMagos")]
        public IActionResult GetRemovendoMagos()
        {            
            personagens.RemoveAll(p => p.Classe == ClasseEnum.Mago);
            return Ok(personagens);
        }

        [HttpGet("GetByInteligencia/{valor}")]
        public IActionResult GetByInteligencia(int valor)
        {
            List<Personagem> listaBusca = personagens.FindAll(p => p.Inteligencia == valor);

            if(listaBusca.Count == 0)            
                return BadRequest("Nenhum personagem encontrado");            
            else
                return Ok(listaBusca);
        }


        [HttpPost]
        public IActionResult AddPersonagem(Personagem novoPersonagem)
        {
            if(novoPersonagem.Inteligencia == 0)
                return BadRequest("O valor da inteligência não pode ser igual a zero.");

            personagens.Add(novoPersonagem);
            return Ok(personagens);
        }

        [HttpPut]
        public IActionResult UpdatePersonagem(Personagem p)
        {
            Personagem personagemAlterado = personagens.Find(pers => pers.Id == p.Id);
            
            personagemAlterado.Nome = p.Nome;
            personagemAlterado.PontosVida = p.PontosVida;
            personagemAlterado.Forca = p.Forca;
            personagemAlterado.Defesa = p.Defesa;
            personagemAlterado.Inteligencia = p.Inteligencia;
            personagemAlterado.Classe = p.Classe;

            return Ok(personagens);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            personagens.RemoveAll(pers => pers.Id == id);

            return Ok(personagens);
        }






        


    }
}