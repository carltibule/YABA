import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import Container from 'react-bootstrap/Container';

export function Footer(props) {
    return (
        <footer className="py-4 bg-light mt-auto">
            <Container fluid>
                <div className="text-center align-middle">YABA: Yet Another Bookmark App</div>
            </Container>
        </footer>
    );
}