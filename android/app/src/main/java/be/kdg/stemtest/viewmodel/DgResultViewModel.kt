package be.kdg.stemtest.viewmodel

import androidx.lifecycle.LiveData
import androidx.lifecycle.ViewModel
import be.kdg.stemtest.model.entity.*
import be.kdg.stemtest.model.repositories.ConnectionRepository
import be.kdg.stemtest.model.repositories.PartyRepository
import be.kdg.stemtest.model.repositories.ResultRepository
import be.kdg.stemtest.model.repositories.StatementRepository
import javax.inject.Inject

class DgResultViewModel @Inject constructor(private val resultRepository: ResultRepository,
                                            private val partyRepository: PartyRepository,
                                            private val statementRepository: StatementRepository,
                                            private val connectionRepository: ConnectionRepository
) : ViewModel() {

    fun getResult(): LiveData<List<DebateGameResult>> {
            return resultRepository.getDebateGameResult()
        }

    fun getParties(): LiveData<List<Party>> {
        return partyRepository.getParties()
    }

    fun getPartyAnswers(): LiveData<List<PartyAnswer>> {
        return resultRepository.getAllPartyAnswers()
    }

    fun getAnswers(): LiveData<List<StudentAnswer>> {
        return resultRepository.getAnswers()
    }

    fun getStatements(): LiveData<List<Statement>> {
        return statementRepository.getAllStatements()
    }

    fun getAnswerOptions(): LiveData<List<AnswerOption>> {
        return statementRepository.getAllAnswerOptions()
    }

    fun getIdentification(): LiveData<Identification> {
        return connectionRepository.getIdentification()
    }
}
