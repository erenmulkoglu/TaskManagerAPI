import { useState } from 'react'

function TaskForm({ onCreate }) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')

  const handleSubmit = (e) => {
    e.preventDefault()
    if (!title.trim()) return
    onCreate(title, description)
    setTitle('')
    setDescription('')
  }

  return (
    <form onSubmit={handleSubmit} className="task-form">
      <input
        type="text"
        placeholder="Görev Başlığı"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        required
      />
      <input
        type="text"
        placeholder="Açıklama (opsiyonel)"
        value={description}
        onChange={(e) => setDescription(e.target.value)}
      />
      <button type="submit">+ Ekle</button>
    </form>
  )
}

export default TaskForm