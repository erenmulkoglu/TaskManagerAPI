import { useState } from 'react'

function TaskList({ tasks, onToggle, onDelete, onEdit }) {
  const [editingId, setEditingId] = useState(null)
  const [editTitle, setEditTitle] = useState('')
  const [editDesc, setEditDesc] = useState('')

  const startEdit = (task) => {
    setEditingId(task.id)
    setEditTitle(task.title)
    setEditDesc(task.description || '')
  }

  const cancelEdit = () => {
    setEditingId(null)
    setEditTitle('')
    setEditDesc('')
  }

  const submitEdit = async (id) => {
    if (!editTitle.trim()) return
    await onEdit(id, editTitle, editDesc)
    cancelEdit()
  }

  if (tasks.length === 0) {
    return <p className="empty">Gorev bulunamadi.</p>
  }

  return (
    <ul className="task-list">
      {tasks.map(task => (
        <li key={task.id} className={`task-item ${task.isCompleted ? 'completed' : ''}`}>
          {editingId === task.id ? (
            // Inline edit modu
            <div className="edit-mode">
              <input
                value={editTitle}
                onChange={e => setEditTitle(e.target.value)}
                placeholder="Baslik"
                autoFocus
              />
              <input
                value={editDesc}
                onChange={e => setEditDesc(e.target.value)}
                placeholder="Aciklama"
              />
              <div className="edit-actions">
                <button className="save-btn" onClick={() => submitEdit(task.id)}>Kaydet</button>
                <button className="cancel-btn" onClick={cancelEdit}>Iptal</button>
              </div>
            </div>
          ) : (
            // Normal mod
            <div className="task-row">
              <div className="task-info">
                <input
                  type="checkbox"
                  checked={task.isCompleted}
                  onChange={() => onToggle(task)}
                />
                <div>
                  <span className="task-title">{task.title}</span>
                  {task.description && (
                    <span className="task-desc">{task.description}</span>
                  )}
                </div>
              </div>
              <div className="task-actions">
                <button className="edit-btn" onClick={() => startEdit(task)}>Duzenle</button>
                <button className="delete-btn" onClick={() => onDelete(task.id)}>Sil</button>
              </div>
            </div>
          )}
        </li>
      ))}
    </ul>
  )
}

export default TaskList