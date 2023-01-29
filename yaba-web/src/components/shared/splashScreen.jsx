import React from "react";
import Spinner from 'react-bootstrap/Spinner';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Button from "react-bootstrap/Button";

export function SplashScreen(props) {
    return(
        <div style={{
                position: "absolute",
                width: "100%",
                height: "100%",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                opacity: "0.6",
                backgroundColor: "grey"
            }}
        >
            <Row style={{
                width: "100%"
            }}>
                <Col xs="12" style={{
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center"
                }}>
                    <Spinner animation="border" />
                </Col>
                <Col xs="12"style={{
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center"
                }}>
                    <div>{props.message}</div>
                </Col>
                <Col xs="12"style={{
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center"
                }}>
                    {props.showCloseSplashScreenButton && (
                        <Button variant="link" onClick={props.onCloseSpashScreenClick}>Close this window</Button>
                    )}
                </Col>
            </Row>
        </div>
    )
}