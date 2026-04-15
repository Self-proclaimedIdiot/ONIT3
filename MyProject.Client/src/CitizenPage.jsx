import React, { useState, useEffect } from 'react';

const CitizenPage = () => {
    const [citizens, setCitizens] = useState([]);
    const [formData, setFormData] = useState({ id: 0, firstName: '', lastName: '', patronim: '' });
    const [isEditing, setIsEditing] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');

    // Твоя "фича" — прямой URL к бэку
    const API_URL = 'api/citizen';

    // Эффект для загрузки данных
    useEffect(() => {
        let isMounted = true;

        const loadData = async () => {
            try {
                // Формируем URL с параметром поиска, если он есть
                const url = searchTerm
                    ? `${API_URL}?name=${encodeURIComponent(searchTerm)}`
                    : API_URL;

                const response = await fetch(url);
                const data = await response.json();

                if (isMounted) {
                    setCitizens(data);
                }
            } catch (error) {
                console.error("Ошибка загрузки:", error);
            }
        };

        loadData();

        return () => {
            isMounted = false;
        };
    }, [searchTerm]); // Линтер теперь доволен: зависимость только searchTerm

    // Вспомогательная функция для обновления списка после CRUD операций
    const refreshData = async () => {
        const url = searchTerm ? `${API_URL}?name=${encodeURIComponent(searchTerm)}` : API_URL;
        const response = await fetch(url);
        const data = await response.json();
        setCitizens(data);
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const method = isEditing ? 'PUT' : 'POST';
        const url = isEditing ? `${API_URL}/${formData.id}` : API_URL;

        await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(formData)
        });

        resetForm();
        refreshData(); // Обновляем список
    };

    const handleDelete = async (id) => {
        if (window.confirm('Вы уверены?')) {
            await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
            refreshData();
        }
    };

    const handleEdit = (citizen) => {
        setFormData(citizen);
        setIsEditing(true);
    };

    const resetForm = () => {
        setFormData({ id: 0, firstName: '', lastName: '', patronim: '' });
        setIsEditing(false);
    };

    return (
        <div style={{ padding: '20px' }}>
            <h2>Управление гражданами</h2>

            <div style={{ marginBottom: '20px' }}>
                <input
                    type="text"
                    placeholder="Поиск по имени..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    style={{ padding: '8px', width: '300px' }}
                />
            </div>

            <form onSubmit={handleSubmit} style={{ marginBottom: '20px' }}>
                <input name="lastName" placeholder="Фамилия" value={formData.lastName} onChange={handleInputChange} required />
                <input name="firstName" placeholder="Имя" value={formData.firstName} onChange={handleInputChange} required />
                <input name="patronim" placeholder="Отчество" value={formData.patronim} onChange={handleInputChange} />
                <button type="submit">{isEditing ? 'Обновить' : 'Добавить'}</button>
                {isEditing && <button type="button" onClick={resetForm}>Отмена</button>}
            </form>

            <table border="1" cellPadding="10" style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Фамилия</th>
                        <th>Имя</th>
                        <th>Отчество</th>
                        <th>Действия</th>
                    </tr>
                </thead>
                <tbody>
                    {citizens.map(c => (
                        <tr key={c.id}>
                            <td>{c.id}</td>
                            <td>{c.lastName}</td>
                            <td>{c.firstName}</td>
                            <td>{c.patronim}</td>
                            <td>
                                <button onClick={() => handleEdit(c)}>Редактировать</button>
                                <button onClick={() => handleDelete(c.id)}>Удалить</button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default CitizenPage;