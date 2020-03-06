import Vue from 'vue'
import store from '@/store/store'
import App from './App.vue'
import router from './router'
import vuetify from './plugins/vuetify';
import axios from 'axios'

axios.defaults.withCredentials = true;

import '@mdi/font/css/materialdesignicons.min.css'
import 'typeface-roboto/index.css'

Vue.config.productionTip = false;

new Vue({
  router,
  vuetify,
  store,
  render: h => h(App)
}).$mount('#app');
