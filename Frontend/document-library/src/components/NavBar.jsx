import React from 'react';
import { Link } from 'react-router-dom';

const NavBar = () => {
    return (
        <nav className="navbar">
            <div className="nav-container">
                <Link to="/" className="nav-logo">
                    Document Library
                </Link>
                <ul className="nav-menu">
                    <li className="nav-item">
                        <Link to="/documents" className="nav-links">Documents</Link>
                    </li>
                    <li className="nav-item">
                        <Link to="/upload" className="nav-links">Upload</Link>
                    </li>
                </ul>
            </div>
        </nav>
    );
};

export default NavBar;
