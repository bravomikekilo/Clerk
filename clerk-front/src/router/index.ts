import Vue from 'vue'
import VueRouter, {RouteConfig} from 'vue-router'
import IndexView from '../views/Index.vue'
import indexViewRoutes from './indexView'
import ProjectIndexView from "@/views/ProjectIndexView.vue";
import ProjectView from "@/views/ProjectView.vue"
import ExperimentView from "@/views/ExperimentView.vue";
import DebugView from "@/views/DebugView.vue";
import UnAuthView from "@/views/UnAuth.vue";
import ForbiddenView from '@/views/Forbidden.vue'

Vue.use(VueRouter);

const routes: RouteConfig[] = [
    {
        path: '',
        name: 'empty',
        redirect: '/index'
    },
    {
        path: '/',
        name: 'Root',
        redirect: '/index'
    },
    {
        path: '/index',
        name: 'Index',
        component: IndexView,
        children: indexViewRoutes
    },
    {
        path: '/project',
        name: 'Project',
        component: ProjectIndexView,
        meta: {
            keepalive: true,
        }
    },
    {
        path: '/project/:projectId',
        name: 'ProjectById',
        component: ProjectView,
        props: (route) => {
            return {projectId: Number.parseInt(route.params.projectId)};
        }
    },
    {
        path: '/experiment/:experimentId',
        name: 'ExperimentById',
        component: ExperimentView,
        props: (route) => {
            return {experimentId: Number.parseInt(route.params.experimentId)}
        }
    },
    {
        path: '/debug',
        name: 'Debug',
        component: DebugView,
    },
    {
        path: '/unauth',
        name: 'UnAuth',
        component: UnAuthView,
    },
    {
        path: '/forbidden',
        name: 'Forbidden',
        component: ForbiddenView,
    },
    {
        path: '/about',
        name: 'About',
        // route level code-splitting
        // this generates a separate chunk (about.[hash].js) for this route
        // which is lazy-loaded when the route is visited.
        component: () => import(/* webpackChunkName: "about" */ '../views/About.vue')
    }
];

const router = new VueRouter({
    mode: 'history',
    base: '/index.html',
    routes
});

export default router
