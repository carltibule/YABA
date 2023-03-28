import React from "react";
import Container from 'react-bootstrap/Container';
import { Link } from 'react-router-dom';
import Spinner from 'react-bootstrap/Spinner';

export function RedirectView(props) {
    const redirectLink = () => props.redirectLink;

    return(
        <React.Fragment>
            <div style={{flexGrow: 1}}>
                <Container className="text-center">
                    <Spinner animation="border" />
                    <h1>{props.message || "You will be redirected in a few seconds." }</h1>
                    {props.redirectLink && (
                        <>
                            <Link to={redirectLink}>Click here to go now</Link> 
                        </>
                    )}
                </Container>
            </div>
        </React.Fragment>
    );
}