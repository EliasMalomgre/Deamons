package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.model.repositories.PartyRepository
import be.kdg.stemtest.model.repositories.ResultRepository
import be.kdg.stemtest.model.repositories.StatementRepository
import javax.inject.Inject

class ResultaatViewModel @Inject constructor(private val resultRepository: ResultRepository,
                                             private val partyRepository: PartyRepository,
                                             private val statementRepository: StatementRepository) : ViewModel() {



    fun getResult():LiveData<String> {
       return resultRepository.getPartyGameResult()
    }

    fun getParty(): LiveData<Party>{
        return partyRepository.getChosenParty()
    }

    fun getAnswerOptions(): LiveData<List<AnswerOption>> {
        return statementRepository.getAllAnswerOptions()
    }

    fun getAnswers(): LiveData<List<StudentAnswer>> {
        return resultRepository.getAnswers()
    }

    fun getStatements(): LiveData<List<Statement>> {
        return statementRepository.getAllStatements()
    }

    fun getPartyAnswers(): LiveData<List<PartyAnswer>> {
        return resultRepository.getPartyAnswers()
    }
}
