import { useState, useEffect } from 'react'
import TaskList from './components/TaskList'
import TaskForm from './components/TaskForm'
import FilterTabs from './components/FilterTabs'
import './App.css'

const API_BASE = 'https://localhost:44317/api/Tasks'
const PAGE_SIZE = 5

function App() {
  const [tasks, setTasks] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [filter, setFilter] = useState('all')
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)

  const fetchTasks = async (currentPage = page, currentFilter = filter) => {
    setLoading(true)
    setError(null)

    try {
      let url = `${API_BASE}?page=${currentPage}&pageSize=${PAGE_SIZE}`
      if (currentFilter === 'pending') url += '&isCompleted=false'
      if (currentFilter === 'completed') url += '&isCompleted=true'

      const res = await fetch(url)
      if (!res.ok) throw new Error('Sunucu hatası')

      const json = await res.json()

      if (json?.data?.items) {
        setTasks(json.data.items)
        setTotalPages(json.data.totalPages ?? 1)
      } else if (Array.isArray(json)) {
        setTasks(json)
        setTotalPages(1)
      } else {
        setTasks([])
        setTotalPages(1)
      }
    } catch (err) {
      console.error(err)
      setError('Görevler yüklenirken hata oluştu.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchTasks(page, filter)
  }, [page, filter])

  const handleFilterChange = (newFilter) => {
    setFilter(newFilter)
    setPage(1)
  }

  const handleCreate = async (title, description) => {
    try {
      const res = await fetch(API_BASE, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, description })
      })
      if (!res.ok) throw new Error()
      fetchTasks(page, filter)
    } catch {
      setError('Görev eklenemedi')
    }
  }

  const handleToggle = async (task) => {
    try {
      const res = await fetch(`${API_BASE}/${task.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          title: task.title,
          description: task.description,
          isCompleted: !task.isCompleted
        })
      })
      if (!res.ok) throw new Error()
      fetchTasks(page, filter)
    } catch {
      setError('Güncelleme hatası')
    }
  }

  const handleDelete = async (id) => {
    try {
      const res = await fetch(`${API_BASE}/${id}`, { method: 'DELETE' })
      if (!res.ok) throw new Error()
      fetchTasks(page, filter)
    } catch {
      setError('Silme hatası')
    }
  }

  const handleEdit = async (id, title, description) => {
    const task = tasks.find(t => t.id === id)
    try {
      const res = await fetch(`${API_BASE}/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          title,
          description,
          isCompleted: task.isCompleted
        })
      })
      if (!res.ok) throw new Error()
      fetchTasks(page, filter)
    } catch {
      setError('Düzenleme hatası')
    }
  }

  return (
    <div className="container">
      <h1>Görev Yöneticisi</h1>

      <TaskForm onCreate={handleCreate} />
      <FilterTabs filter={filter} onFilterChange={handleFilterChange} />

      {error && <div className="error-box">{error}</div>}

      {loading ? (
        <div className="skeleton-list">
          {[...Array(3)].map((_, i) => (
            <div key={i} className="skeleton-item" />
          ))}
        </div>
      ) : (
        <>
          <TaskList
            tasks={tasks}
            onToggle={handleToggle}
            onDelete={handleDelete}
            onEdit={handleEdit}
          />
          {totalPages > 1 && (
            <div className="pagination">
              <button onClick={() => setPage(p => p - 1)} disabled={page === 1}>
                Önceki
              </button>
              <span>{page} / {totalPages}</span>
              <button onClick={() => setPage(p => p + 1)} disabled={page === totalPages}>
                Sonraki
              </button>
            </div>
          )}
        </>
      )}
    </div>
  )
}

export default App
