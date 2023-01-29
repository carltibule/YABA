import React from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

export function NotFoundView(props) {
    return(
        <React.Fragment>
            <div style={{
                    flexGrow: 1,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    textAlign: "center"
                }}
            >
                <Row>
                    <Col xs="12">
                        <h1>404</h1>
                    </Col>
                    <Col xs="12">
                        <h2>{props.message || "The page you are looking for does not exist or another error has occured"}</h2>
                    </Col>
                </Row>
            </div>
        </React.Fragment>
    );
}