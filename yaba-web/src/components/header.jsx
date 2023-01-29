import React from 'react';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useAuth0 } from "@auth0/auth0-react";

export function Header(props) {
    const { isAuthenticated, loginWithRedirect, logout } = useAuth0();
    const handleLogin = async () => {
        await loginWithRedirect({
          appState: {
            returnTo: "/bookmarks",
          },
        });
      };

    const handleSignUp = async () => {
        await loginWithRedirect({
          appState: {
            returnTo: "/bookmarks",
          },
          authorizationParams: {
            screen_hint: "signup",
          },
        });
      };

    const handleLogout = () => {
        logout({
          logoutParams: {
            returnTo: window.location.origin,
          },
        });
      };

    return (
        <div>
            <Navbar bg="dark" variant="dark" expand="lg">
                <Container>
                    <Navbar.Brand href={!isAuthenticated ? "/" : "/bookmarks"}>YABA: Yet Another Bookmark App</Navbar.Brand>
                    <Navbar.Toggle aria-controls="basic-navbar-nav" />
                    <Navbar.Collapse id="basic-navbar-nav" />
                    <Nav className="ms-auto">
                        {!isAuthenticated && (
                            <>
                                <Nav.Link onClick={handleLogin}>Login</Nav.Link>
                                <Nav.Link onClick={handleSignUp}>Register</Nav.Link>
                            </>
                        )}
                        { isAuthenticated && (
                            <>
                                <NavDropdown title="Bookmarks">
                                    <NavDropdown.Item href="/bookmarks">All</NavDropdown.Item>
                                    <NavDropdown.Item href="/bookmarks/hidden">Hidden</NavDropdown.Item>
                                    <NavDropdown.Divider />
                                    <NavDropdown.Item href="/bookmarks/new">New</NavDropdown.Item>
                                </NavDropdown>
                                <Nav.Link onClick={handleLogout}>Logout</Nav.Link>
                            </>
                        )}
                    </Nav>
                </Container>
            </Navbar>
        </div>
    );
}