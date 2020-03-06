import {RouteConfig} from "vue-router/types/router";
import SignIn from "@/components/SignIn.vue";
import SignUp from "@/components/SignUp.vue";

const indexViewRoutes: RouteConfig[] = [
    {
        path: '',
        name: 'IndexEmpty',
        component: SignIn
    },
    {
        path: 'sign-in',
        name: 'IndexSignIn',
        component: SignIn
    },
    {
        path: "sign-up",
        name: 'IndexSignUp',
        component: SignUp
    }
];

export default indexViewRoutes;
