import axios from 'axios';
axios.defaults.baseURL = process.env.MY_URL;
axios.interceptors.request.use(function (config) {
    const token = 'your-auth-token';
    console.log('Request Config:', config);
    return config;
}, function (error) {

    console.error('Request Error:', error);
    return Promise.reject(error);
});
axios.interceptors.response.use(function (response) {
    console.log('Response Data:', response.data);
    return response;
}, function (error) {
    console.error('Response Error:', error.response ? error.response.data : error.message);
    return Promise.reject(error);
});


export default {
    getTasks: async () => {
        const result = await axios.get(`/items`)
        return result.data;
    },

    addTask: async (name) => {
        console.log('addTask', name)

        const result = await axios.post(`/items`, { name: name, isComplete: false })
        return result;
    },

    setCompleted: async (id, isComplete) => {
        console.log('setCompleted', { id, isComplete })
        const result = await axios.put(`/items/${id}`, { isComplete: isComplete })

        return {};
    },

    deleteTask: async (id) => {
        console.log('deleteTask')
        const result = await axios.delete(`/items/${id}`)

    }
};
