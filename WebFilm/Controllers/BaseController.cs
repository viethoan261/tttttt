using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebFilm.Core.Interfaces.Services;

namespace WebFilm.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TKey, TEntity> : ControllerBase
    {
        #region Field
        IBaseService<TKey, TEntity> _baseService;
        protected readonly static Type EntityType = typeof(TEntity);
        #endregion

        #region Contructor
        public BaseController(IBaseService<TKey, TEntity> baseService)
        {
            _baseService = baseService;
        }
        #endregion
        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    try
        //    {
        //        var entity = _baseService.GetAll();
        //        return Ok(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleException(ex);
        //    }
        //}

        //[HttpGet("{id}")]
        //public IActionResult GetByID(TKey id)
        //{
        //    try
        //    {
        //        var entity = _baseService.GetByID(id);
        //        return Ok(entity);
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleException(ex);
        //    }
        //}

        //[HttpPut("{id}")]
        //public IActionResult Edit(TKey id, TEntity entity)
        //{
        //    try
        //    {
        //        var res = _baseService.Edit(id, entity);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleException(ex);
        //    }
        //}

        //[HttpPost]
        //public IActionResult Add(TEntity entity)
        //{
        //    try
        //    {
        //        var res = _baseService.Add(entity);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleException(ex);
        //    }
        //}

        //[HttpDelete]
        //public IActionResult Delete(TKey id)
        //{
        //    try
        //    {
        //        var res = _baseService.Delete(id);
        //        return Ok(res);
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleException(ex);
        //    }
        //}

        protected IActionResult HandleException(Exception ex)
        {
            var response = new
            {
                devMsg = ex.Message,
                userMsg = Core.Resources.Resource.Error_Exception,
            };
            return StatusCode(500, response);

        }
    }
}
