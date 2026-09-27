using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserStore _store;

    public UsersController(IUserStore store)
    {
        _store = store;
    }

    // GET api/users
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll() => Ok(_store.GetAll());

    // GET api/users/5
    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = _store.Get(id);
        return user is null ? NotFound() : Ok(user);
    }

    // POST api/users
    [HttpPost]
    public ActionResult<User> Add([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest(new { error = "UserName and Email are required." });
        }

        var created = _store.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/users/5
    [HttpPut("{id:int}")]
    public ActionResult<User> Update(int id, [FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest(new { error = "UserName and Email are required." });
        }

        if (!_store.Update(id, user))
        {
            return NotFound();
        }

        return Ok(user);
    }

    // DELETE api/users/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => _store.Delete(id) ? NoContent() : NotFound();
}
