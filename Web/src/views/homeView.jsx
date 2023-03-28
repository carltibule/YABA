import React, { useEffect } from "react";
import Container from 'react-bootstrap/Container';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import { useAuth0 } from "@auth0/auth0-react";
import { useNavigate } from "react-router-dom";

export function HomeView(props) {
    const { isAuthenticated, loginWithRedirect } = useAuth0();
    const navigate = useNavigate();

    const navigateToLogin = async () => {
        await loginWithRedirect({
            appState: {
            returnTo: "/bookmarks",
            },
        });
    };

    useEffect(() => {
        if(isAuthenticated) {
            navigate("/bookmarks");
        } else {
            navigateToLogin();
        }
    }, []);

    return(
        <></>
    );
}