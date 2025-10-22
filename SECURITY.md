# Security Policy

## Overview

This document outlines the security policy for the **Robotech: Macross Era** (TBS) project. As a personal fan project and open-source Unity game, we take security seriously to protect contributors and users.

---

## Supported Versions

Security updates are provided for the following versions:

| Version | Supported          | Status |
| ------- | ------------------ | ------ |
| main    | :white_check_mark: | Active Development |
| dev     | :white_check_mark: | If branch exists |
| < 1.0   | :x:                | Pre-release (no guarantees) |

**Note:** This is a personal project in early development. Security updates will be applied on a best-effort basis.

---

## Reporting a Vulnerability

If you discover a security vulnerability in this project, please follow these steps:

### Preferred Method: GitHub Security Advisories

1. Navigate to the repository's Security tab
2. Click "Report a vulnerability"
3. Fill out the form with as much detail as possible
4. Submit confidentially

**Link:** https://github.com/MatthewSnow2/Robotech-tbs/security/advisories/new

### Alternative Method: Private Contact

If you prefer not to use GitHub Security Advisories, you can contact the maintainer directly:

- **GitHub:** @MatthewSnow2
- **Through GitHub:** Send a direct message via GitHub

### What to Include

When reporting a vulnerability, please include:

- **Description** of the vulnerability
- **Steps to reproduce** the issue
- **Potential impact** of the vulnerability
- **Suggested fix** (if you have one)
- **Unity version** and platform where you found the issue
- **Any relevant code snippets or screenshots**

---

## What NOT to Report Publicly

Please **DO NOT** create public GitHub issues for security vulnerabilities. This includes:

- Authentication bypasses
- Code execution vulnerabilities
- Data exposure issues
- Access control problems
- Any vulnerability that could be exploited maliciously

Public disclosure should only happen **after** the vulnerability has been patched.

---

## Security Best Practices for Contributors

If you're contributing to this project, please follow these security guidelines:

### 1. Never Commit Secrets

- **Never commit** API keys, passwords, or credentials
- Use `.env` files for local secrets (already in `.gitignore`)
- Use Unity's `PlayerPrefs` or `ScriptableObjects` for configuration

### 2. Code Review

- All pull requests should be reviewed for security issues
- Look for potential vulnerabilities like:
  - Hardcoded credentials
  - Insecure random number generation
  - Potential save file tampering
  - Network security issues (if multiplayer is added)

### 3. Dependencies

- Keep Unity and packages up to date
- Review security advisories for Unity and third-party assets
- Only use trusted Unity Asset Store packages

### 4. Input Validation

- Always validate user input
- Sanitize file paths for save/load operations
- Validate network messages (if multiplayer is added)

---

## Security Features in This Project

### Current Security Measures

- ✅ **Comprehensive `.gitignore`** - Prevents accidental commit of sensitive files
- ✅ **No hardcoded credentials** - All configurations use proper Unity patterns
- ✅ **Secure code patterns** - No SQL injection, command injection, or deserialization risks
- ✅ **Clean git history** - No accidentally committed secrets

### Planned Security Features

As the project grows, we plan to implement:

- [ ] Save file integrity validation
- [ ] Secure multiplayer communication (if added)
- [ ] Mod sandboxing (if modding support is added)
- [ ] Anti-cheat measures (if competitive multiplayer is added)

---

## Known Security Considerations

### Current State (Early Development)

This project is in early development and currently has:

- **No network communication** - Offline single-player game
- **No save file system** - Not yet implemented
- **No user authentication** - Not applicable for single-player
- **No external API calls** - Pure Unity/C# project

As these features are added, appropriate security measures will be implemented.

### Future Considerations

If these features are added, we will address:

1. **Multiplayer Security**
   - Authoritative server model
   - Input validation
   - Cheat detection
   - Rate limiting

2. **Save File Security**
   - Integrity checking
   - Tampering detection
   - Encryption for sensitive data

3. **Modding Security**
   - Mod validation
   - Sandboxed execution
   - Allowlist-based permissions

4. **Analytics/Telemetry**
   - Privacy-respecting data collection
   - User consent
   - Data anonymization

---

## Security Audit History

| Date       | Type           | Findings       | Status   |
|------------|----------------|----------------|----------|
| 2025-10-21 | Automated Scan | No critical issues | ✅ Clear |
| 2025-10-21 | Code Review    | Minor code quality improvements | ✅ Fixed |

---

## Response Timeline

When you report a security vulnerability:

- **Acknowledgment:** Within 48 hours
- **Initial Assessment:** Within 7 days
- **Fix Timeline:** Varies by severity
  - **Critical:** Immediate (within 24-48 hours)
  - **High:** Within 1 week
  - **Medium:** Within 2 weeks
  - **Low:** Within 30 days or next release

**Note:** As a personal project, these timelines are best-effort estimates.

---

## Disclosure Policy

We follow **coordinated disclosure**:

1. Vulnerability is reported privately
2. We confirm and investigate the issue
3. We develop and test a fix
4. We release the fix
5. Public disclosure occurs **after** the fix is available

We request that reporters:

- Give us reasonable time to fix the issue (typically 90 days)
- Avoid exploiting the vulnerability
- Avoid public disclosure until the fix is released

---

## Security Updates

Security updates will be announced via:

- **GitHub Releases** - For version updates with security fixes
- **GitHub Security Advisories** - For critical vulnerabilities
- **README.md** - For important security notices

---

## Contact

For security concerns or questions about this policy:

- **GitHub:** @MatthewSnow2
- **Security Advisories:** https://github.com/MatthewSnow2/Robotech-tbs/security/advisories

---

## License and Legal

This project is a **non-commercial fan project** for the Robotech IP.

- **No warranty** is provided for the security or functionality of this software
- Use at your own risk
- See LICENSE file for full terms

---

## Acknowledgments

We appreciate responsible security researchers who help keep this project safe:

- *Your name could be here!*

If you report a security vulnerability, we'll acknowledge your contribution (with your permission) after the fix is released.

---

**Last Updated:** 2025-10-21

**Policy Version:** 1.0

---

*Thank you for helping keep Robotech: Macross Era secure!*
