function FilterTabs({ filter, onFilterChange }) {
  const tabs = [
    { key: 'all', label: 'Tumü' },
    { key: 'pending', label: 'Bekleyenler' },
    { key: 'completed', label: 'Tamamlananlar' },
  ]

  return (
    <div className="filter-tabs">
      {tabs.map(tab => (
        <button
          key={tab.key}
          className={`tab-btn ${filter === tab.key ? 'active' : ''}`}
          onClick={() => onFilterChange(tab.key)}
        >
          {tab.label}
        </button>
      ))}
    </div>
  )
}

export default FilterTabs