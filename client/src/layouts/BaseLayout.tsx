import { Outlet } from "react-router";
import Footer from "../components/Footer";

export default function BaseLayout() {
    return (
        <>
            <Outlet />
            <Footer />
        </>
    )
}