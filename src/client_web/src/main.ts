import { createApp } from 'vue'
import 'animate.css';
import App from '@/App.vue'
import { registerPlugins } from '@core/utils/plugins'

// Styles
import 'tippy.js/dist/tippy.css'
import 'tippy.js/themes/light.css'
import 'tippy.js/themes/light-border.css'
import '@core/scss/template/index.scss'
import '@layouts/styles/index.scss'
import 'leaflet/dist/leaflet.css';
import '@styles/scrollbar.scss';

// Create vue app
const app = createApp(App)

// Register plugins
registerPlugins(app)

// Mount vue app
app.mount('#app')
