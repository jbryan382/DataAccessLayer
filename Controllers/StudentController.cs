using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;
using DataAccessLayer.ViewModels;

namespace DataAccessLayer.Controllers
{
    // All of these routes will be at the base URL:     /api/Student
    // That is what "api/[controller]" means below. It uses the name of the controller
    // in this case StudentController to determine the URL
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        // This is the variable you use to have access to your database
        private readonly DatabaseContext _context;

        // Constructor that recives a reference to your database context
        // and stores it in _context for you to use in your API methods
        public StudentController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Student
        //
        // Returns a list of all your Students
        //
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            // Uses the database context in `_context` to request all of the Students, sort
            // them by row id and return them as a JSON array.
            return await _context.Students.OrderBy(row => row.ID).ToListAsync();
        }

        // GET: api/Student/5
        //
        // Fetches and returns a specific student by finding it by id. The id is specified in the
        // URL. In the sample URL above it is the `5`.  The "{id}" in the [HttpGet("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            // Find the student in the database using `FindAsync` to look it up by id
            var student = await _context.Students.FindAsync(id);

            // If we didn't find anything, we receive a `null` in return
            if (student == null)
            {
                // Return a `404` response to the client indicating we could not find a student with this id
                return NotFound();
            }

            //  Return the student as a JSON object.
            return student;
        }

        // PUT: api/Student/5
        //
        // Update an individual student with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpPut("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        // In addition the `body` of the request is parsed and then made available to us as a Student
        // variable named student. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Student POCO class. This represents the
        // new values for the record.
        //
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            // If the ID in the URL does not match the ID in the supplied request body, return a bad request
            if (id != student.ID)
            {
                return BadRequest();
            }

            // Tell the database to consider everything in student to be _updated_ values. When
            // the save happens the database will _replace_ the values in the database with the ones from student
            _context.Entry(student).State = EntityState.Modified;

            try
            {
                // Try to save these changes.
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Ooops, looks like there was an error, so check to see if the record we were
                // updating no longer exists.
                if (!StudentExists(id))
                {
                    // If the record we tried to update was already deleted by someone else,
                    // return a `404` not found
                    return NotFound();
                }
                else
                {
                    // Otherwise throw the error back, which will cause the request to fail
                    // and generate an error to the client.
                    throw;
                }
            }

            // Return a copy of the updated data
            return Ok(student);
        }

        // POST: api/Student
        //
        // Creates a new student in the database.
        //
        // The `body` of the request is parsed and then made available to us as a Student
        // variable named student. The controller matches the keys of the JSON object the client
        // supplies to the names of the attributes of our Student POCO class. This represents the
        // new values for the record.
        //

        [BindProperty]
        public StudentViewModel StudentVM { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            var entry = _context.Add(new Student());
            entry.CurrentValues.SetValues(StudentVM);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }


        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            // Indicate to the database context we want to add this new record
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Return a response that indicates the object was created (status code `201`) and some additional
            // headers with details of the newly created object.
            return CreatedAtAction("GetStudent", new { id = student.ID }, student);
        }

        // DELETE: api/Student/5
        //
        // Deletes an individual student with the requested id. The id is specified in the URL
        // In the sample URL above it is the `5`. The "{id} in the [HttpDelete("{id}")] is what tells dotnet
        // to grab the id from the URL. It is then made available to us as the `id` argument to the method.
        //
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            // Find this student by looking for the specific id
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                // There wasn't a student with that id so return a `404` not found
                return NotFound();
            }

            // Tell the database we want to remove this record
            _context.Students.Remove(student);

            // Tell the database to perform the deletion
            await _context.SaveChangesAsync();

            // Return a copy of the deleted data
            return Ok(student);
        }

        // Private helper method that looks up an existing student by the supplied id
        private bool StudentExists(int id)
        {
            return _context.Students.Any(student => student.ID == id);
        }
    }
}
