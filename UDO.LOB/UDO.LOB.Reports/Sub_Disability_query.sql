SELECT  udo_Percentage perc,
        udo_Disability disab,
        udo_DiagnosticCode codes,
        udo_EffectiveDate eff
FROM udo_lettergenerationdisability serreq
WHERE (serreq.udo_lettergenerationId = @LetterGenerationGUID)
ORDER BY perc DESC