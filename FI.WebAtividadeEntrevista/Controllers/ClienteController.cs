using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using Microsoft.Ajax.Utilities;
using System.Web.Services.Description;
using System.Reflection;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        private static List<Beneficiario> beneficiarios = new List<Beneficiario>();

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            beneficiarios.Clear();
            return View();
        }

        [HttpPost]
        public JsonResult AddBeneficiario(Beneficiario beneficiario)
        {
            BoBeneficiarios bo = new BoBeneficiarios();
            if (bo.ValidarCPF(beneficiario.CPF))
            {
                if (beneficiario.IdCliente > 0)
                {
                    if (!bo.VerificarExistencia(beneficiario.CPF, beneficiario.IdCliente) && !beneficiarioExiste(beneficiario.CPF))
                    {
                        beneficiarios.Add(beneficiario);
                        return Json(new { success = true, data = beneficiarios });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Beneficiário já existente para este cliente. Tente outro." });
                    }
                }
                else
                {
                    if (beneficiarioExiste(beneficiario.CPF))
                    {
                        return Json(new { success = false, message = "Beneficiário já existente para este cliente. Tente outro." });

                    }
                    else
                    {
                        beneficiarios.Add(beneficiario);
                        return Json(new { success = true, data = beneficiarios });
                    }
                }

            }
            else
            {
                return Json(new { success = false, message = "Digite um CPF Válido." });
            }
        }

        bool beneficiarioExiste(string cpfBeneficiario)
        {
            var ben = beneficiarios.FirstOrDefault(b => b.CPF == cpfBeneficiario);
            return ben != null;

        }

        [HttpPost]
        public JsonResult BeneficiarioList(long idCliente)
        {
            try
            {
                List<Beneficiario> ben = new BoBeneficiarios().Listar(idCliente);
                beneficiarios = ben;
                return Json(ben, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult RemoveBeneficiario(BeneficiarioModel model)
        {
            BoBeneficiarios boBen = new BoBeneficiarios();
            var beneficiario = beneficiarios.FirstOrDefault(b => b.Id == model.Id);
            if (model.Id > 0)
            {
                boBen.Excluir(model.Id);
            }
            beneficiarios.Remove(beneficiario);

            return Json(beneficiarios);
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (bo.ValidarCPF(model.CPF))
            {
                if (!bo.VerificarExistencia(model.CPF))
                {
                    BoBeneficiarios boBen = new BoBeneficiarios();

                    if (!this.ModelState.IsValid)
                    {
                        List<string> erros = (from item in ModelState.Values
                                              from error in item.Errors
                                              select error.ErrorMessage).ToList();

                        Response.StatusCode = 400;
                        return Json(string.Join(Environment.NewLine, erros));
                    }
                    else
                    {

                        model.Id = bo.Incluir(new Cliente()
                        {
                            CEP = model.CEP,
                            CPF = model.CPF,
                            Cidade = model.Cidade,
                            Email = model.Email,
                            Estado = model.Estado,
                            Logradouro = model.Logradouro,
                            Nacionalidade = model.Nacionalidade,
                            Nome = model.Nome,
                            Sobrenome = model.Sobrenome,
                            Telefone = model.Telefone
                        });
                        if (beneficiarios.Count > 0)
                        {
                            foreach (Beneficiario beneficiario in beneficiarios)
                            {
                                beneficiario.IdCliente = model.Id;
                                boBen.Incluir(beneficiario);
                            }
                        }
                        return Json(new { success = true, message = "Cadastro efetuado com sucesso" });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "CPF Já existe. Tente outro." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Digite um CPF Válido." });
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();
            BoBeneficiarios boBen = new BoBeneficiarios();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    CPF = model.CPF,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone
                });
                if (beneficiarios.Count > 0)
                {
                    foreach (Beneficiario beneficiario in beneficiarios)
                    {
                        if (beneficiario.Id == 0)
                        {
                            boBen.Incluir(beneficiario);
                        }
                    }
                }
                return Json(new { success = true, message = "Cadastro alterado com sucesso" });
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;
            beneficiarios = new BoBeneficiarios().Listar(id);
            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    CPF = cliente.CPF,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone
                };

            }

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}