import React, { useEffect, useState } from 'react'
import Content from './components/Content'
import Header from './components/Header'
import axios from 'axios'
import './custom.scss'

export function App() {
  const [students, setStudents] = useState([])

  useEffect(() => {
    const response = axios('/api/Student').then((resp) =>
      setStudents(resp.data)
    )
    console.log('Response:', response)
  }, [])

  return (
    <div className="d-flex justify-content-center display-2">
      <Header />
      <div class="container">
        <main role="main" class="pb-3">
          <Content />
          <div>
            <h1>Details</h1>

            <div>
              <h4>Student</h4>
              <hr />
              {
                !!students.map((student) => {
                  return (
                    <dl class="row">
                      <dt class="col-sm-2">Last Name</dt>
                      <dd class="col-sm-10">{student.LastName}</dd>
                      <dt class="col-sm-2">First Mid Name</dt>
                      <dd class="col-sm-10">{student.FirstMidName}</dd>
                      <dt class="col-sm-2">Enrollment Date</dt>
                      <dd class="col-sm-10">{student.EnrollmentDate}</dd>
                      {student.Enrollments.map((enroll) => {
                        return (
                          <dd class="col-sm-10">
                            <table class="table">
                              <tr>
                                <th>Course Title</th>
                                <th>Grade</th>
                              </tr>
                              <tr>
                                <td>{enroll.Course}</td>
                                <td>{enroll.Grade}</td>
                              </tr>
                            </table>
                          </dd>
                        )
                      })}
                    </dl>
                  )
                })
              }
            </div>
            <div>
              <a asp-page="./Edit" asp-route-id="@Model.Student.ID">
                Edit
              </a>{' '}
              |<a asp-page="./Index">Back to List</a>
            </div>
          </div>
        </main>
      </div>

      <footer class="border-top footer text-muted">
        <div class="container">
          &copy; 2019 - Contoso University -{' '}
          <a asp-area="" asp-page="/Privacy">
            Privacy
          </a>
        </div>
      </footer>
    </div>
  )
}
